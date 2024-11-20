using AuctionService.Helper;
using AuctionService.Models;

namespace AuctionService.IRepository
{
    public interface ISoldLotRepository
    {
        Task<List<SoldLot>> GetAllAsync(SoldLotQueryObject query);
        Task<SoldLot> GetSoldLotById(int auctionLotId);
        Task<SoldLot> CreateSoldLot(SoldLot soldLot);

    }
}