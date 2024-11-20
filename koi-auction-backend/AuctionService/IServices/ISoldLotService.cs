using AuctionService.Dto.SoldLot;
using AuctionService.Helper;
using AuctionService.Models;

namespace AuctionService.IServices
{
    public interface ISoldLotService
    {
        Task<List<SoldLotDto>> GetAllAsync(SoldLotQueryObject queryObject);
        Task<SoldLotDto> GetSoldLotById(int id);
        Task<SoldLot> CreateSoldLot(CreateSoldLotDto soldLotDto);
    }
}