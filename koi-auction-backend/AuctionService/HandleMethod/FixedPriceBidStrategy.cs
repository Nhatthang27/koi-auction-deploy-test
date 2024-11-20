using System.Collections.Concurrent;
using AuctionService.Dto.AuctionLot;
using AuctionService.Dto.BidLog;
using AuctionService.Dto.SoldLot;
using AuctionService.IServices;
using AuctionService.Mapper;
using AuctionService.Models;
using AuctionService.Services;

namespace AuctionService.HandleMethod
{
    public class FixedPriceBidStrategy : ABidStrategyService
    {
        private readonly List<CreateBidLogDto> _bids; // list de chon winner
        private readonly ConcurrentDictionary<int, bool> _isPlacedBid; // list de chon winner

        public FixedPriceBidStrategy()
        : base()
        {
            _bids = new();
            _isPlacedBid = new();
        }

        public override HighestBidLog? GetWinner()
        {
            if (_bids == null || _bids.Count == 0)
                return null;

            Random random = new Random();
            int randomIndex = random.Next(_bids.Count);
            return _bids[randomIndex].ToHighestBidLogFromCreateBidLogDto();
            //random winner in dictionary
        }


        public override bool IsBidValid(CreateBidLogDto bid, AuctionLotBidDto? auctionLotBidDto)
        {
            // System.Console.WriteLine($"bid {auctionLotBidDto!.AuctionLotId} = {bid.AuctionLotId} start price = {auctionLotBidDto.StartPrice}");
            //kiểm tra AuctionLotStaus
            if (auctionLotBidDto != null
                    && auctionLotBidDto!.AuctionLotId == bid.AuctionLotId
                    && bid.BidAmount == auctionLotBidDto.StartPrice
                    && _isPlacedBid.ContainsKey(bid.BidderId) == false)
            {

                _bids.Add(bid);
                _isPlacedBid.TryAdd(bid.BidderId, true);
                return true;
            }
            return false;
        }
        //test
        // public void PrintAllBids()
        // {
        //     foreach (var bid in _bids)
        //     {
        //         Console.WriteLine($"Bid ID: {bid.BidderId}, Amount: {bid.BidAmount}");
        //     }
        // }

    }
}