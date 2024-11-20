using System.Collections.Concurrent;
using System.Text.Json;
using AuctionService.Dto.Address;
using AuctionService.Dto.AuctionDeposit;
using AuctionService.Dto.AuctionLot;
using AuctionService.Dto.BidLog;
using AuctionService.Dto.ScheduledTask;
using AuctionService.Dto.SoldLot;
using AuctionService.Dto.UserConnection;
using AuctionService.Dto.Wallet;
using AuctionService.HandleMethod;
using AuctionService.Helper;
using AuctionService.Hubs;
using AuctionService.IRepository;
using AuctionService.IServices;
using Microsoft.AspNetCore.SignalR;

namespace AuctionService.Services
{
    public class BidManagementService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private ConcurrentDictionary<int, BidService> _bidServices;
        public ConcurrentDictionary<int, BidService> BidServices
        {
            get => _bidServices;
            private set => _bidServices = value;
        }
        public ConcurrentDictionary<int, IServiceScope> _auctionLotScopes;
        private readonly IDictionary<string, UserConnectionDto> _connections;
        private readonly IHubContext<BidHub> _bidHub;
        private readonly HttpClient _httpClient;
        private const int EXP_TIME = 5;

        public BidManagementService(HttpClient httpClient, IServiceScopeFactory serviceScopeFactory, IHubContext<BidHub> bidHub, IDictionary<string, UserConnectionDto> connections)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _auctionLotScopes = new ConcurrentDictionary<int, IServiceScope>();
            _bidServices = new ConcurrentDictionary<int, BidService>();
            _bidHub = bidHub;
            _connections = connections;
            _httpClient = httpClient;
        }

        // Bắt đầu phiên đấu giá và khởi tạo PlaceBidService trong scope riêng
        public async Task StartAuctionLotAsync(AuctionLotBidDto auctionLotBidDto)
        {
            Console.WriteLine($"BidManagement: Receive auction lot bid dto {auctionLotBidDto.AuctionLotId}");
            // if (_serviceScope != null)
            // {
            //     _serviceScope.Dispose();
            //     _serviceScope = null;
            //     _bidService = null;
            // }
            // Tạo scope mới cho phiên đấu giá
            _auctionLotScopes[auctionLotBidDto.AuctionLotId] = _serviceScopeFactory.CreateScope();
            _bidServices[auctionLotBidDto.AuctionLotId] = _auctionLotScopes[auctionLotBidDto.AuctionLotId].ServiceProvider.GetRequiredService<BidService>();

            BidService curBidService = _bidServices[auctionLotBidDto.AuctionLotId];

            curBidService.AuctionLotBidDto = auctionLotBidDto;
            curBidService.SetStrategy(auctionLotBidDto);

            string auctionLotId = auctionLotBidDto.AuctionLotId.ToString();

            // await _bidHub.Clients.All.SendAsync(WsMess.ReceiveStartAuctionLot);
            // await _bidHub.Clients.All.SendAsync(WsMess.ReceiveFetchAuctionLot);
            // await _bidHub.Clients.All.SendAsync(WsMess.ReceivePredictEndTime, curBidService.GetPredictEndTime());
            await _bidHub.Clients.Group(auctionLotId).SendAsync(WsMess.ReceiveStartAuctionLot);
            await _bidHub.Clients.Group(auctionLotId).SendAsync(WsMess.ReceiveFetchAuctionLot);
            await _bidHub.Clients.Group(auctionLotId).SendAsync(WsMess.ReceivePredictEndTime, curBidService.GetPredictEndTime());

            if (auctionLotBidDto.AuctionMethodId == (int)Enums.BidMethodType.AscendingBid)
            {
                // await _bidHub.Clients.All.SendAsync(WsMess.ReceiveWinner, curBidService.GetWinner());
                await _bidHub.Clients.Group(auctionLotId).SendAsync(WsMess.ReceiveWinner, curBidService.GetWinner());
            }
            if (auctionLotBidDto.AuctionMethodId == (int)Enums.BidMethodType.DescendingBid)
            {
                // await _bidHub.Clients.All.SendAsync(WsMess.ReceivePriceDesc, curBidService.GetPriceDesc());
                await _bidHub.Clients.Group(auctionLotId).SendAsync(WsMess.ReceivePriceDesc, curBidService.GetPriceDesc());
            }
        }

        public async Task EndAuctionLotAsync(int auctionLotId)
        {
            System.Console.WriteLine($"Bidding Management Service: End auction lot {auctionLotId}");
            if (!_auctionLotScopes.ContainsKey(auctionLotId) || !_bidServices.ContainsKey(auctionLotId))
            {
                System.Console.WriteLine("Bidding Management Service: Auction lot is not started yet");
                return;
            }
            BidService curBidService = _bidServices[auctionLotId];
            var winner = curBidService.GetWinner();

            //send loading
            // await _bidHub.Clients.All.SendAsync(WsMess.ReceiveLoading);
            await _bidHub.Clients.Group(auctionLotId.ToString()).SendAsync(WsMess.ReceiveLoading);

            if (winner == null) System.Console.WriteLine($"Winner for auction lot {auctionLotId}: No winner");
            else
                System.Console.WriteLine($"Winner for auction lot {auctionLotId}: {winner!.BidderId}");

            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    var lot = await unitOfWork.Lots.GetLotByIdAsync(curBidService.AuctionLotBidDto!.AuctionLotId);
                    lot.LotStatusId = winner == null ? (int)Enums.LotStatus.UnSold : (int)Enums.LotStatus.ToPay;

                    if (winner != null)
                    {
                        var winnerAddressResponse = await _httpClient.GetAsync($"http://localhost:3000/user-service/manage/profile/address/{winner.BidderId}");
                        var content = await winnerAddressResponse.Content.ReadAsStringAsync();
                        var addressDto = JsonSerializer.Deserialize<AddressDto>(content);

                        var soldLot = new Models.SoldLot
                        {
                            SoldLotId = curBidService.AuctionLotBidDto!.AuctionLotId,
                            WinnerId = winner.BidderId,
                            FinalPrice = winner.BidAmount,
                            BreederId = lot.BreederId,
                            Address = addressDto?.Address,
                            // Address = winner.Address, get from user service
                            ExpTime = DateTime.Now.AddMinutes(EXP_TIME)
                        };
                        await unitOfWork.SoldLot.CreateSoldLot(soldLot);

                        var taskSchedulerService = scope.ServiceProvider.GetRequiredService<ITaskSchedulerService>();
                        taskSchedulerService.ScheduleTask(new ScheduledTask
                        {
                            ExecuteAt = soldLot.ExpTime,
                            Task = async () => await HandlePaymentOverdue(soldLot.SoldLotId, soldLot.WinnerId)
                        });
                        //send websocket to winner
                        // 
                        string winnerConnectionId = _connections.FirstOrDefault(x => x.Value.UserId == winner.BidderId).Key;
                        await _bidHub.Clients.Client(winnerConnectionId).SendAsync(WsMess.ReceivePendingPayment, soldLot);

                    }

                    var auctionDepositService = scope.ServiceProvider.GetRequiredService<IAuctionDepositService>();

                    //call user service to update user wallet
                    var penRefundList = await auctionDepositService.GetAuctionDepositByStatus(curBidService.AuctionLotBidDto!.AuctionLotId, Enums.AuctionDepositStatus.PendingRefund);
                    penRefundList.RemoveAll(a => a.UserId == winner?.BidderId);
                    List<RefundDto> refundList = new List<RefundDto>();
                    foreach (var auctionDeposit in penRefundList)
                    {
                        refundList.Add(new RefundDto
                        {
                            UserId = auctionDeposit.UserId,
                            Amount = auctionDeposit.Amount,
                            Description = $"Refund for auction lot {auctionDeposit.AuctionLotId}"
                        });
                    }
                    var walletService = scope.ServiceProvider.GetRequiredService<WalletService>();
                    await walletService.RefundAsync(refundList);

                    await auctionDepositService.UpdateRefundedStatus(curBidService.AuctionLotBidDto!.AuctionLotId, winner?.BidderId ?? -1);

                    await unitOfWork.SaveChangesAsync();
                    await _bidHub.Clients.Group(auctionLotId.ToString()).SendAsync(WsMess.ReceiveWinner, winner);
                    await _bidHub.Clients.Group(auctionLotId.ToString()).SendAsync(WsMess.ReceiveFetchAuctionLot);
                    await _bidHub.Clients.Group(auctionLotId.ToString()).SendAsync(WsMess.ReceiveFetchBidLog);
                    await _bidHub.Clients.Group(auctionLotId.ToString()).SendAsync(WsMess.ReceiveFetchWinnerPrice);
                    await _bidHub.Clients.Group(auctionLotId.ToString()).SendAsync(WsMess.ReceiveEndAuctionLot);
                }
            }
            catch (Exception e)
            {
                System.Console.Error.WriteLine(e.Message);
            }
            // Dispose và reset các giá trị
            _auctionLotScopes[auctionLotId].Dispose();
            _auctionLotScopes.TryRemove(auctionLotId, out _);
            _bidServices.TryRemove(auctionLotId, out _);
        }


        private async Task HandlePaymentOverdue(int soldLotId, int winnerId)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var lot = await unitOfWork.Lots.GetLotByIdAsync(soldLotId);
                if (lot == null)
                {
                    System.Console.WriteLine($"Sold lot {soldLotId} not found");
                    return;
                }
                if (lot.LotStatusId != (int)Enums.LotStatus.ToPay)
                {
                    System.Console.WriteLine($"Sold lot {soldLotId} is not in ToPay status");
                    return;
                }
                lot.LotStatusId = (int)Enums.LotStatus.PaymentOverdue;

                var auctionDeposit = await unitOfWork.AuctionDeposits.GetAuctionDepositByAuctionLotIdAndUserId(winnerId, soldLotId);
                if (auctionDeposit == null)
                {
                    System.Console.WriteLine($"Auction deposit of user {winnerId} in sold lot {soldLotId} not found");
                    return;
                }
                auctionDeposit.AuctionDepositStatus = Enums.AuctionDepositStatus.DepositForfeiture;
                await unitOfWork.SaveChangesAsync();
            }
        }
    }
}

// await _bidHub.Clients.All.SendAsync(WsMess.ReceiveWinner, winner);
// await _bidHub.Clients.All.SendAsync(WsMess.ReceiveFetchAuctionLot);
// await _bidHub.Clients.All.SendAsync(WsMess.ReceiveFetchBidLog);
// await _bidHub.Clients.All.SendAsync(WsMess.ReceiveFetchWinnerPrice);
// await _bidHub.Clients.All.SendAsync(WsMess.ReceiveEndAuctionLot);

// _serviceScope.Dispose();
// _serviceScope = null;
// _bidService = null;