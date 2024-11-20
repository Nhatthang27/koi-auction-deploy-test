using System.Collections.Concurrent;
using AuctionService.Dto.AuctionLot;
using AuctionService.Dto.BidLog;
using AuctionService.IServices;
using AuctionService.Mapper;
using AuctionService.Services;

namespace AuctionService.HandleMethod
{
    public class SealedBidStrategy : ABidStrategyService
    {
        private readonly List<CreateBidLogDto> _bids;
        private readonly ConcurrentDictionary<int, bool> _isPlacedBid; // list de chon winner

        private decimal? _highestBid;
        public SealedBidStrategy()
        : base() // Truyền bidService đến constructor của lớp cha
        {
            _bids = new();
            _isPlacedBid = new();
        }
        public override HighestBidLog? GetWinner()
        {
            if (_bids == null || _bids.Count == 0)
                return null;
            else
                return _bids[0].ToHighestBidLogFromCreateBidLogDto();


        }

        public override bool IsBidValid(CreateBidLogDto bid, AuctionLotBidDto? auctionLotBidDto)
        {
            if (auctionLotBidDto == null || auctionLotBidDto.AuctionLotId != bid.AuctionLotId
                || bid.BidAmount < auctionLotBidDto.StartPrice
                || _isPlacedBid.ContainsKey(bid.BidderId))
            {
                return false;
            }

            // Cập nhật _highestBid và kiểm tra xem bid có hợp lệ không
            if (_highestBid == null || bid.BidAmount > _highestBid)
            {
                _highestBid = bid.BidAmount;
                _bids.Clear(); // Xóa các bid có cùng giá trị với _highestBid trước đó
                _bids.Add(bid);
            }
            _isPlacedBid.TryAdd(bid.BidderId, true);
            System.Console.WriteLine($"bid.BiddedId: {bid.BidderId} method 2 -> bid oke {bid.BidAmount}");
            return true;
        }

    }
}