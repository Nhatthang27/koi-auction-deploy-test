using AuctionService.Dto.AuctionLot;
using AuctionService.Dto.BidLog;
using AuctionService.IServices;


namespace AuctionService.HandleMethod
{
    public abstract class ABidStrategyService : IBidStrategy
    {

        protected ABidStrategyService()
        {
        }

        public abstract bool IsBidValid(CreateBidLogDto bid, AuctionLotBidDto? auctionLotBidDto);

        public abstract HighestBidLog? GetWinner();

        //method to be called when the countdown timer is finished
    }
}