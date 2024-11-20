using AuctionService.Models;
using AuctionService.Dto.Auction;
using AuctionService.Helper;

namespace AuctionService.IRepository
{
    public interface IAuctionRepository
    {
        Task<List<Auction>> GetAllAsync(AuctionQueryObject queryObject);
        Task<Auction> GetByIdAsync(int id);
        Task<Auction> CreateAsync(Auction auction);
        Task<Auction> UpdateAsync(int id, UpdateAuctionDto updateAuctionDto);
        Task<Auction> DeleteAsync(int id);

        //Update status of auction
        Task<Auction?> UpdateStatusAsync(int id, int auctionStatusId);

        Task<Auction?> UpdateEndTimeAsync(int id, DateTime endTime);

    }
}