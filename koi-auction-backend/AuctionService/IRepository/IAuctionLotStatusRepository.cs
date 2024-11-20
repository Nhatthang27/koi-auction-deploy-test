using AuctionService.Models;

namespace AuctionService.IRepository
{
    public interface IAuctionLotStatusRepository
    {
        Task<List<AuctionLotStatus>> GetAllAsync();
        Task<AuctionLotStatus> GetAuctionLotStatusByIdAsync(int id);
    }
}