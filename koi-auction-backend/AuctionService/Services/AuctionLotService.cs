using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionService.Dto.AuctionLot;
using AuctionService.Dto.Lot;
using AuctionService.Dto.ScheduledTask;
using AuctionService.Enums;
using AuctionService.HandleMethod;
using AuctionService.Helper;
using AuctionService.IRepository;
using AuctionService.IServices;
using AuctionService.Mapper;
using AuctionService.Models;

namespace AuctionService.Services
{
    public class AuctionLotService : IAuctionLotService
    {
        private IAuctionService _auctionService;
        private readonly BidManagementService _bidManagementService;
        private readonly ITaskSchedulerService _taskSchedulerService;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IUnitOfWork _unitOfWork;
        public AuctionLotService(IUnitOfWork unitOfWork, IAuctionService auctionService, BidManagementService bidManagementService, ITaskSchedulerService taskSchedulerService, IServiceScopeFactory serviceScopeFactory)
        {
            _bidManagementService = bidManagementService;
            _taskSchedulerService = taskSchedulerService;
            _serviceScopeFactory = serviceScopeFactory;
            _auctionService = auctionService;
            _unitOfWork = unitOfWork;
        }

        public async Task<AuctionLot> DeleteAsync(int id)
        {
            var auctionLot = await _unitOfWork.AuctionLots.GetAuctionLotById(id);
            // Kiểm tra trạng thái AuctionLot
            if (auctionLot == null)
                throw new KeyNotFoundException($"Auction Lot with ID {id} was not found.");
            var tmp = (int)Enums.AuctionLotStatus.Upcoming;
            System.Console.WriteLine($"tmp = {tmp}");
            if (auctionLot.AuctionLotStatusId != (int)Enums.AuctionLotStatus.Upcoming)
                throw new InvalidOperationException("AuctionLot Status must be Upcoming to delete.");
            // Xóa AuctionLot nếu trạng thái hợp lệ
            await _unitOfWork.AuctionLots.DeleteAsync(id);
            // Cập nhật trạng thái Lot nếu cần
            await _unitOfWork.Lots.UpdateLotStatusAsync(auctionLot.AuctionLotId,
                new UpdateLotStatusDto { LotStatusName = "Approved" });
            await _unitOfWork.SaveChangesAsync();
            return auctionLot;
        }
        public async Task<bool> DeleteListAsync(List<int> ids)
        {
            var deletedAuctionLots = await _unitOfWork.AuctionLots.DeleteListAsync(ids);
            foreach (var auctionLot in deletedAuctionLots)
            {
                await _unitOfWork.Lots.UpdateLotStatusAsync(auctionLot.AuctionLotId,
                                            new UpdateLotStatusDto { LotStatusName = "Approved" });
            }
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        public async Task<AuctionLot> CreateAsync(CreateAuctionLotDto createAuctionLot)
        {
            var auctionLot = createAuctionLot.ToAuctionLotFromCreateAuctionLotDto();
            await _unitOfWork.Lots.UpdateLotStatusAsync(auctionLot.AuctionLotId,
                                            new UpdateLotStatusDto { LotStatusName = "In auction" });
            var newAuctionLot = await _unitOfWork.AuctionLots.CreateAsync(auctionLot);
            await _unitOfWork.SaveChangesAsync();
            return newAuctionLot;
        }
        public async Task<List<AuctionLot>> CreateListAsync(List<CreateAuctionLotDto> auctionLotDtos)
        {
            var auctionLots = auctionLotDtos.Select(dto => dto.ToAuctionLotFromCreateAuctionLotDto()).ToList();
            List<int> auctionLotIds = auctionLots.Select(a => a.AuctionLotId).ToList();
            await _unitOfWork.Lots.UpdateLotsStatusToInAuctionAsync(auctionLotIds);
            await _unitOfWork.AuctionLots.CreateListAsync(auctionLots);
            await _unitOfWork.SaveChangesAsync();
            return auctionLots;
        }

        public async Task ScheduleAuctionLotAsync(int auctionLotId, DateTime startTime)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                IUnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                AuctionLot auctionLot = await unitOfWork.AuctionLots.GetAuctionLotById(auctionLotId);
                auctionLot.StartTime = startTime;
                auctionLot.AuctionLotStatusId = (int)Enums.AuctionLotStatus.Scheduled;

                if (!await unitOfWork.SaveChangesAsync())
                {
                    throw new Exception("An error occurred while saving the data");
                }
            }
            _taskSchedulerService.ScheduleTask(new ScheduledTask
            {
                ExecuteAt = startTime,
                Task = async () => await StartAuctionLotAsync(auctionLotId)
            });
            System.Console.WriteLine($"Auction lot {auctionLotId} is scheduled to start at {startTime}");
        }
        public async Task StartAuctionLotAsync(int auctionLotId)
        {
            Console.WriteLine($"Auction lot {auctionLotId} is starting!");
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                IUnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                AuctionLot auctionLot = await unitOfWork.AuctionLots.GetAuctionLotById(auctionLotId);
                auctionLot.AuctionLotStatusId = (int)Enums.AuctionLotStatus.Ongoing;

                if (!await unitOfWork.SaveChangesAsync())
                {
                    throw new Exception("An error occurred while saving the data");
                }

                //set up auction bid dto in bid service
                AuctionLotBidDto auctionLotBidDto = auctionLot.ToAuctionLotBidDtoFromAuctionLot();
                await _bidManagementService.StartAuctionLotAsync(auctionLotBidDto);

                DateTime endTimePredict = auctionLot.StartTime!.Value.Add(auctionLot.Duration);
                // Schedule extended time auction lot
                switch (auctionLotBidDto.AuctionMethodId)
                {
                    case (int)BidMethodType.FixedPrice:
                        _taskSchedulerService.ScheduleTask(new ScheduledTask
                        {
                            ExecuteAt = endTimePredict,
                            Task = async () => await EndAuctionLotAsync(auctionLotId)
                        });
                        break;
                    case (int)BidMethodType.SealedBid:
                        _taskSchedulerService.ScheduleTask(new ScheduledTask
                        {
                            ExecuteAt = endTimePredict,
                            Task = async () => await EndAuctionLotAsync(auctionLotId)
                        });
                        break;
                    case (int)BidMethodType.AscendingBid:
                        if (_bidManagementService.BidServices[auctionLotId].CurrentStrategy is AscendingBidStrategy strategyAsc)
                        {
                            strategyAsc.CountdownFinished += async (id) => await EndAuctionLotAsync(auctionLotId);
                        }
                        break;
                    case (int)BidMethodType.DescendingBid:
                        if (_bidManagementService.BidServices[auctionLotId].CurrentStrategy is DescendingBidStrategy strategyDesc)
                        {
                            strategyDesc.CountdownFinished += async (id) => await EndAuctionLotAsync(auctionLotId);
                        }
                        break;
                    // Thêm các case khác ở đây nếu có
                    default:
                        throw new ArgumentException("Invalid auctionLotMethodId");
                }
            }
        }
        public async Task EndAuctionLotAsync(int auctionLotId)
        {
            Console.WriteLine($"Auction lot {auctionLotId} is ending!");
            AuctionLot? auctionLot = null;
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                //unit of work
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                auctionLot = await unitOfWork.AuctionLots.GetAuctionLotById(auctionLotId);
                auctionLot.AuctionLotStatusId = (int)Enums.AuctionLotStatus.Ended;
                auctionLot.EndTime = DateTime.Now;
                await unitOfWork.SaveChangesAsync();
                await _bidManagementService.EndAuctionLotAsync(auctionLotId);
                if (!await unitOfWork.AuctionLots.IsAuctionLotInAuction(auctionLot.AuctionId))
                {
                    await _auctionService.EndAuctionAsync(auctionLot.AuctionId);
                }
            }
        }
        public Task<List<AuctionLotBidDto>> SearchAuctionLot(AuctionLotQueryObject queryObject)
        {
            throw new NotImplementedException();
        }
    }
}

/*
ScheduleAuctionLot 
    lên lịch để StartActionLot
    cập nhật startTime của AuctionLot trong cơ sở dữ liệu
    cập nhật trạng thái thành scheduled

StartAuctionLot sẽ cập nhật 
    status, 
    set up auction bid dto in bid service
    thông báo qua SignalR -> auction lot is starting

EndAuctionLot sẽ cập nhật
    status,
    endTime của AuctionLot trong cơ sở dữ liệu
    thông báo qua SignalR -> auction lot is ending
*/

// private const int EXTENDED_TIME = 20;


// public void ScheduleExentedPhase(int auctionLotId, DateTime startExtendedTime)
// {
//     // _jobScheduler.Schedule(() => StartExtendedPhase(auctionLotId), startExtendedTime);
//     _taskSchedulerService.ScheduleTask(new ScheduledTask
//     {
//         ExecuteAt = startExtendedTime,
//         Action = () => StartExtendedPhase(auctionLotId)
//     });
//     System.Console.WriteLine($"Auction lot {auctionLotId} is scheduled to start extended phase at {startExtendedTime}");
// }
// public void StartExtendedPhase(int auctionLotId)
// {
//     _bidManagementService.BidService!.CountdownFinished += (id) => EndAuctionLotAsync(auctionLotId);
//     _bidManagementService.BidService!.StartExtendPhase();
// }