
using AuctionService.Dto.ScheduledTask;
using AuctionService.Enums;
using AuctionService.Helper;
using AuctionService.IRepository;
using AuctionService.IServices;
using AuctionService.Models;

namespace AuctionService.Services
{
    public class AuctionService : IAuctionService
    {
        private readonly ITaskSchedulerService _taskSchedulerService;

        private readonly IServiceScopeFactory _serviceScopeFactory;

        public AuctionService(ITaskSchedulerService taskSchedulerService, IServiceScopeFactory serviceScopeFactory)
        {
            _taskSchedulerService = taskSchedulerService;
            _serviceScopeFactory = serviceScopeFactory;
        }


        public void ScheduleAuction(int auctionId, DateTime startTime)
        {
            _taskSchedulerService.ScheduleTask(new ScheduledTask
            {
                ExecuteAt = startTime,
                Task = async () => await StartAuctionAsync(auctionId)
            });
        }

        public async Task StartAuctionAsync(int auctionId)
        {
            System.Console.WriteLine($"Auction {auctionId} is starting!");
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                Auction auction = await unitOfWork.Auctions.GetByIdAsync(auctionId);
                auction.AuctionStatusId = (int)Enums.AuctionStatus.Ongoing;
                await unitOfWork.SaveChangesAsync();
            }
        }

        public async Task EndAuctionAsync(int auctionId)
        {
            System.Console.WriteLine($"Auction {auctionId} is ending!");
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                Auction auction = await unitOfWork.Auctions.GetByIdAsync(auctionId);
                auction.AuctionStatusId = (int)Enums.AuctionStatus.Ended;
                auction.EndTime = DateTime.Now;
                await unitOfWork.SaveChangesAsync();
            }
        }

        // {
        //     _unitOfWork = unitOfWork;
        //     _auctionLotService = auctionLotService;
        // }

        // public void ScheduleAuction(int auctionId, DateTime startTime)
        // {
        //     TimeSpan timeToStart = startTime - DateTime.Now;
        //     var jobId = BackgroundJob.Schedule(() => StartAuction(auctionId, startTime), timeToStart);
        // }

        // public async Task StartAuction(int auctionId, DateTime startTime)
        // {
        //     // Logic bắt đầu phiên đấu giá
        //     Console.WriteLine($"Auction {auctionId} is starting!");

        //     // Cập nhật trạng thái phiên đấu giá trong cơ sở dữ liệu
        //     await _unitOfWork.Auctions.UpdateStatusAsync(auctionId, AUCTION_STATUS_ONGOING);
        //     // Get auction lot has order in auction = 1
        //     var auctionLot = await _unitOfWork.AuctionLots.GetAuctionLotByOrderInAuction(auctionId, FIRST_AUCTION_LOT_ORDER);

        //     if (auctionLot != null)
        //         await _auctionLotService.ScheduleAuctionLot(auctionLot.AuctionLotId, startTime);

        //     if (!await _unitOfWork.SaveChangesAsync())
        //     {
        //         throw new Exception("An error occurred while saving the data");
        //     }

        //     // Thông báo qua SignalR hoặc các phương thức khác
        // }


    }
}
