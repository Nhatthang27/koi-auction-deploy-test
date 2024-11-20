using AuctionService.Dto.AuctionLot;
using AuctionService.Dto.BidLog;
using AuctionService.Helper;
using AuctionService.Hubs;
using AuctionService.IServices;
using AuctionService.Mapper;
using AuctionService.Models;
using AuctionService.Services;
using Microsoft.AspNetCore.SignalR;

namespace AuctionService.HandleMethod
{
    public class DescendingBidStrategy : ABidStrategyService
    {
        private decimal? _softCap;
        private HighestBidLog? _winner = null;
        private readonly Timer _timer;
        private IHubContext<BidHub> _bidHub;
        private AuctionLotBidDto? _auctionLotBidDto;
        private decimal? _currentPrice;
        public decimal? CurrentPrice => _currentPrice;
        private decimal? _stepPrice;
        public event Func<int, Task>? CountdownFinished;

        private const int _decreaseInterval = 20;

        public DescendingBidStrategy(IHubContext<BidHub> bidHub)
        : base() // Truyền bidService đến constructor của lớp cha
        {
            // Khởi tạo giá hiện tại nếu chưa có(bắt đầu với giá khởi điểm)
            System.Console.WriteLine($"DescendingBidStrategy constructor called");
            // Initialize and start the timer to decrease the price every minute
            _timer = new Timer(DecreasePrice, null, TimeSpan.FromSeconds(_decreaseInterval - 1), TimeSpan.FromSeconds(_decreaseInterval - 1));
            _bidHub = bidHub;
        }

        public void SetUp(AuctionLotBidDto auctionLotBidDto)
        {
            System.Console.WriteLine($"SetUp called");
            _auctionLotBidDto = auctionLotBidDto;
            _currentPrice = auctionLotBidDto.StartPrice;
            _softCap = _currentPrice / 2;
            _stepPrice = auctionLotBidDto.StepPercent * auctionLotBidDto.StartPrice / 100;
        }

        public override HighestBidLog? GetWinner()
        {
            return _winner;
        }

        public override bool IsBidValid(CreateBidLogDto bid, AuctionLotBidDto? auctionLotBidDto)
        {
            if (auctionLotBidDto == null || auctionLotBidDto.AuctionLotId != bid.AuctionLotId)
            {
                return false;
            }

            System.Console.WriteLine($"currentPrice = {_currentPrice}");

            // Kiểm tra nếu BidAmount đạt các tiêu chí và người mua có đủ số dư
            if (bid.BidAmount == _currentPrice)
            {
                _winner = bid.ToHighestBidLogFromCreateBidLogDto(); // Cập nhật người thắng cuộc là người đầu tiên chấp nhận giá hiện tại
                _timer.Dispose();
                Task.Run(() => CountdownFinished!.Invoke(auctionLotBidDto.AuctionLotId)); // Kết thúc phiên đấu giá
                return true;
            }

            return false;
        }

        private void DecreasePrice(object? state)
        {
            if (_auctionLotBidDto != null)
            {
                System.Console.WriteLine($"Remaining time = {_auctionLotBidDto.RemainingTime}");
                _auctionLotBidDto.RemainingTime -= TimeSpan.FromSeconds(_decreaseInterval);
                _currentPrice -= _stepPrice;

                if (_auctionLotBidDto.RemainingTime.TotalSeconds <= 0 || _currentPrice <= _softCap)
                {
                    _timer.Dispose();
                    Task.Run(() => CountdownFinished!.Invoke(_auctionLotBidDto.AuctionLotId));
                }
                // _bidHub.Clients.All.SendAsync(WsMess.ReceivePriceDesc, _currentPrice);
                _bidHub.Clients.Group(_auctionLotBidDto.AuctionLotId.ToString()).SendAsync(WsMess.ReceivePriceDesc, _currentPrice);
            }
            System.Console.WriteLine($"Updated currentPrice = {_currentPrice}");
        }
    }
}