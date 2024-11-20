using AuctionService.Models;
using AuctionService.Dto.AuctionLot;
using AuctionService.Dto.LotStatus;

namespace AuctionService.IRepository
{
    public interface ILotStatusRepository
    {
        Task<List<LotStatus>> GetAllAsync();
        Task<LotStatus?> GetLotStatusByIdAsync(int id);
        // Task<LotStatus?> GetLotStatusByLotIdAsync(int lotId);
        Task<LotStatus> CreateLotStatusAsync(LotStatus LotStatus);
        Task<LotStatus> UpdateLotStatusAsync(int id, UpdateStatusDto lotStatusDto);
        Task<LotStatus> DeleteLotStatusAsync(int id);
    }
}