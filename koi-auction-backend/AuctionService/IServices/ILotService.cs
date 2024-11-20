using AuctionService.Dto.Dashboard;
using AuctionService.Dto.Lot;
using AuctionService.Helper;

namespace AuctionService.IServices
{
  public interface ILotService
  {
    Task<List<LotAuctionMethodStatisticDto>> GetLotAuctionMethodStatisticAsync();
    Task<List<BreederStatisticDto>> GetBreederStatisticsAsync();
    Task<TotalDto> GetTotalLotsStatisticsAsync(int? breederId, DateTime startDateTime, DateTime endDateTime);
    Task<List<LotSearchResultDto>> GetLotSearchResults(int breederId);
    // Task<List<DailyRevenueDto>> GetWeeklyRevenueOfBreeders(int year, int month, int weekOfMonth);
    Task<List<DailyRevenueDto>> GetStatisticsRevenue(DateTime startDateTime, DateTime endDateTime);

  }
}