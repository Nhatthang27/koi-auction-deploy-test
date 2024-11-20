using AuctionService.Models;
using AuctionService.Dto.AuctionLot;
using AuctionService.Helper;

namespace AuctionService.IRepository
{
    public interface IAuctionLotRepository
    {
        Task<List<AuctionLot>> GetAllAsync(AuctionLotQueryObject query);
        Task<AuctionLot> GetAuctionLotById(int id);
        Task<AuctionLot> CreateAsync(AuctionLot auctionLot);
        Task<List<AuctionLot>> CreateListAsync(List<AuctionLot> auctionLots);
        AuctionLot Update(AuctionLot auctionLot, UpdateAuctionLotDto updateAuctionLotDto);
        Task<AuctionLot> DeleteAsync(int id);
        Task<List<AuctionLot>> DeleteListAsync(List<int> ids);

        Task<AuctionLot?> UpdateStatusAsync(int id, int statusId);

        Task<AuctionLot?> UpdateStartTimeAsync(int id, DateTime startTime);

        Task<AuctionLot?> UpdateEndTimeAsync(int id, DateTime endTime);

        Task<AuctionLot?> GetAuctionLotByOrderInAuction(int auctionId, int orderInAuction);

        Task<bool> IsAuctionLotInAuction(int auctionId);

    }
}