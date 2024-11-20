using AuctionService.Dto.Dashboard;
using AuctionService.Dto.Lot;
using AuctionService.Helper;
using AuctionService.IRepository;
using AuctionService.IServices;
using AuctionService.Models;

namespace AuctionService.Services
{
    public class LotService : ILotService
    {
        private readonly int REJECT = 3;
        private readonly int UNSOLD = 5;
        private readonly int TOSHIP = 7;
        private readonly int TORECEIVE = 8;
        private readonly int COMPLETED = 9;
        private readonly int CANCELED = 10;
        private readonly IUnitOfWork _unitOfWork;
        private readonly BreederDetailService _breederService;

        public LotService(IUnitOfWork unitOfWork, BreederDetailService breederService)
        {
            _unitOfWork = unitOfWork;
            _breederService = breederService;
        }

        public async Task<List<LotAuctionMethodStatisticDto>> GetLotAuctionMethodStatisticAsync()
        {
            return await _unitOfWork.Lots.GetLotAuctionMethodStatisticAsync();
        }

        public async Task<List<BreederStatisticDto>> GetBreederStatisticsAsync()
        {
            var breeders = await _breederService.GetAllBreederAsync();
            var lots = await _unitOfWork.Lots.GetBreederLotsStatisticsAsync();
            var statistics = new List<BreederStatisticDto>();

            var breederGroups = lots.GroupBy(l => l.BreederId).ToDictionary(g => g.Key, g => g.ToList()); ;

            foreach (var breeder in breeders)
            {
                if (breederGroups.TryGetValue(breeder.BreederId, out var group))
                {

                    // var breeder = await _breederService.GetBreederByIdAsync(group.Key);
                    // if (breeder == null) continue;
                    // Tính các thống kê cho breeder có lot đấu giá

                    //Total auction lots
                    var totalLots = group.Count(l =>
                      l.AuctionLot != null
                    );

                    // Unsold lots are those with "Unsold" status
                    var unsoldLots = group.Count(l => l.LotStatus!.LotStatusId == UNSOLD);

                    // Cancelled sold lots are those with "Canceled" status AND have a SoldLot record
                    var cancelledSoldLots = group.Count(l =>
                        l.LotStatus!.LotStatusId == CANCELED &&
                        l.AuctionLot != null &&
                        l.AuctionLot.SoldLot != null);

                    // Completed lots are those with "Completed" status and have a SoldLot record
                    var completedLots = group.Count(l =>
                        l.LotStatus!.LotStatusId == COMPLETED &&
                        l.AuctionLot != null &&
                        l.AuctionLot.SoldLot != null);

                    statistics.Add(new BreederStatisticDto
                    {
                        BreederId = breeder.BreederId,
                        FarmName = breeder.FarmName!,
                        TotalAuctionLot = totalLots,
                        CountSuccess = completedLots,
                        CountUnsuccess = unsoldLots + cancelledSoldLots,
                        PercentUnsold = CalculatePercentage(unsoldLots, totalLots),
                        PercentCancelledSoldLot = CalculatePercentage(cancelledSoldLots, totalLots),
                        PercentSuccess = CalculatePercentage(completedLots, totalLots),
                        PercentUnsuccess = CalculatePercentage(unsoldLots + cancelledSoldLots, totalLots),
                        Priority = GetPriorityBasedOnSuccessRate(CalculatePercentage(completedLots, totalLots))
                    });
                }
                else
                {
                    // Thêm breeder vào thống kê với các giá trị mặc định khi không có lô đấu giá
                    statistics.Add(new BreederStatisticDto
                    {
                        BreederId = breeder.BreederId,
                        FarmName = breeder.FarmName!,
                        TotalAuctionLot = 0,
                        CountSuccess = 0,
                        CountUnsuccess = 0,
                        PercentUnsold = 0,
                        PercentCancelledSoldLot = 0,
                        PercentSuccess = 0,
                        PercentUnsuccess = 0,
                        Priority = 3
                    });
                }
            }
            return statistics.OrderByDescending(s => s.PercentSuccess).ToList();
        }

        private double CalculatePercentage(int part, int total)
        {
            if (total == 0) return 0;
            return Math.Round((double)part / total * 100, 2);
        }

        // Phương thức để xác định priority dựa trên tỷ lệ thành công
        private int GetPriorityBasedOnSuccessRate(double percentSuccess)
        {
            if (percentSuccess >= 70)
                return 1;
            else if (percentSuccess >= 40)
                return 2;
            else
                return 3;
        }

        // fixed -- trả về 6 ô thông tin cho FE
        // thống kê tình hình tổng số lượng của auctionLot theo khoảng thời gian
        public async Task<TotalDto> GetTotalLotsStatisticsAsync(int? breederId, DateTime startDateTime, DateTime endDateTime)
        {
            // Tạo điều kiện lọc Lot theo breederId
            var queryObject = new LotQueryObject { BreederId = breederId };
            var lots = await _unitOfWork.Lots.GetAllAsync(queryObject);

            endDateTime = endDateTime.AddDays(1).AddSeconds(-1);
            // Lọc Lot trong khoảng thời gian startDateTime và endDateTime
            var filteredLots = lots.Where(l =>
                l.UpdatedAt >= startDateTime &&
                l.UpdatedAt <= endDateTime).ToList();
            System.Console.WriteLine($"starttime {startDateTime}, endtime {endDateTime}");
            System.Console.WriteLine($"filter {filteredLots.Count}");
            var total = filteredLots.Count;

            // Đếm số lượng completed lots
            var completedLots = filteredLots.Count(l =>
                l.LotStatus != null &&
                l.LotStatus.LotStatusId == COMPLETED &&
                l.AuctionLot != null &&
                l.AuctionLot.SoldLot != null);

            // Đếm số lượng toShipLots
            var toShipLots = filteredLots.Count(l =>
                l.LotStatus != null &&
                l.LotStatus.LotStatusId == TOSHIP &&
                l.AuctionLot != null &&
                l.AuctionLot.SoldLot != null);

            // Đếm số lượng toReceiveLots
            var toReceiveLots = filteredLots.Count(l =>
                l.LotStatus != null &&
                l.LotStatus.LotStatusId == TORECEIVE &&
                l.AuctionLot != null &&
                l.AuctionLot.SoldLot != null);

            // Đếm số lượng unsold lots
            var unsoldLots = filteredLots.Count(l =>
                l.LotStatus != null &&
                l.LotStatus.LotStatusId == UNSOLD);

            // Đếm số lượng cancelled sold lots
            var cancelledSoldLots = filteredLots.Count(l =>
                l.LotStatus != null &&
                l.LotStatus.LotStatusId == CANCELED &&
                l.AuctionLot != null &&
                l.AuctionLot.SoldLot != null);

            // Đếm số lượng rejected lots
            var rejectLot = filteredLots.Count(l =>
                l.LotStatus != null &&
                l.LotStatus.LotStatusId == REJECT);

            // Tạo đối tượng TotalDto với kết quả
            var result = new TotalDto
            {
                Total = total,
                CompletedLots = completedLots > 0 ? completedLots : 0,
                UnsoldLots = unsoldLots > 0 ? unsoldLots : 0,
                ToShipLots = toShipLots > 0 ? toShipLots : 0,
                ToReceiveLots = toReceiveLots > 0 ? toReceiveLots : 0,
                CanceledSoldLots = cancelledSoldLots > 0 ? cancelledSoldLots : 0,
                RejectedLots = rejectLot > 0 ? rejectLot : 0
            };

            if (result == null)
            {
                throw new InvalidOperationException("Total Statistics Fail");
            }

            return result;
        }


        public async Task<List<LotSearchResultDto>> GetLotSearchResults(int breederId)
        {
            var result = await _unitOfWork.Lots.GetLotSearchResults(breederId) ?? throw new InvalidOperationException("Not lot search existed");
            return result;
        }

        // fixed -- trả về biểu đồ đường
        public async Task<List<DailyRevenueDto>> GetStatisticsRevenue(DateTime startDateTime, DateTime endDateTime)
        {
            var result = await _unitOfWork.Lots.GetStatisticsRevenue(startDateTime, endDateTime);
            return result;
        }

        // public async Task<List<DailyRevenueDto>> GetWeeklyRevenueOfBreeders(int year, int month, int weekOfMonth)
        // {
        //     var revenue = await _unitOfWork.Lots.GetWeeklyRevenueOfBreeders(year, month, weekOfMonth);
        //     return revenue;
        // }

    }
}