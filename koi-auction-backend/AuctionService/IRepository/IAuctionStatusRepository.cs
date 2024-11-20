using AuctionService.Models;

namespace AuctionService.IRepository
{
    public interface IAuctionStatusRepository
    {
        Task<List<AuctionStatus>> GetAllAsync();
        Task<AuctionStatus> GetAuctionStatusByIdAsync(int id);

    }
}