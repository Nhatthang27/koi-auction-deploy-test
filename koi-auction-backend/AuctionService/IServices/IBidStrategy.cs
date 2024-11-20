using AuctionService.Dto.AuctionLot;
using AuctionService.Dto.BidLog;
using AuctionService.Dto.SoldLot;
using AuctionService.Models;

namespace AuctionService.IServices
{
    public interface IBidStrategy
    {
        bool IsBidValid(CreateBidLogDto bid, AuctionLotBidDto? auctionLotBidDto);

        HighestBidLog? GetWinner();
    }
}