using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionService.Models;
using AuctionService.Dto.Lot;
using AuctionService.Dto.LotRequestForm;
using AuctionService.IRepository;
using Microsoft.EntityFrameworkCore;
using AuctionService.Helper;
using AuctionService.Data;
using AuctionService.Mapper;
using AuctionService.Dto.KoiMedia;
using AuctionService.Dto.Dashboard;

namespace AuctionService.Repository
{
    public class LotRepository : ILotRepository
    {
        private readonly int COMPLETED = 9;
        private readonly int PAYMENT_OVERDUE = 11;
        private readonly AuctionManagementDbContext _context;

        public LotRepository(AuctionManagementDbContext context)
        {
            _context = context;
        }
        public async Task<Lot> CreateLotAsync(Lot lot)
        {
            var status = await _context.LotStatuses
                                     .FirstOrDefaultAsync(ls => ls.LotStatusName == "Pending");
            lot.LotStatusId = status!.LotStatusId;
            await _context.LotStatuses.FindAsync(lot.LotStatusId);
            await _context.AuctionMethods.FindAsync(lot.AuctionMethodId);
            await _context.Lots.AddAsync(lot);

            return lot;
        }

        public async Task<Lot> DeleteLotAsync(int id)
        {
            var lot = await _context.Lots.Include(l => l.KoiFish).FirstOrDefaultAsync(l => l.LotId == id);
            if (lot == null)
                throw new KeyNotFoundException($" Lot {id} was not found");
            var status = await _context.LotStatuses
                                  .FirstOrDefaultAsync(ls => ls.LotStatusName == "Canceled");
            lot.LotStatusId = status!.LotStatusId;
            await _context.AuctionMethods.FindAsync(lot.AuctionMethodId);

            return lot;
        }


        public async Task<List<Lot>> GetAllAsync(LotQueryObject query)
        {
            var lots = _context.Lots.Include(l => l.KoiFish).ThenInclude(m => m!.KoiMedia).
                                        Include(l => l.LotStatus).Include(l => l.AuctionLot).Include(l => l.AuctionLot!.SoldLot)
                                        .Include(l => l.AuctionMethod).AsQueryable();

            // Kiểm tra BreederId
            if (query.BreederId.HasValue)
            {
                lots = lots.Where(l => l.BreederId == query.BreederId.Value);
            }

            // Kiểm tra StartingPrice
            if (query.StartingPrice.HasValue)
            {
                lots = lots.Where(l => l.StartingPrice >= query.StartingPrice.Value);
            }

            // Kiểm tra LotStatusId
            if (query.LotStatusId.HasValue)
            {
                lots = lots.Where(l => l.LotStatusId == query.LotStatusId.Value);
            }

            // Kiểm tra AuctionMethodId
            if (query.AuctionMethodId.HasValue)
            {
                lots = lots.Where(l => l.AuctionMethodId == query.AuctionMethodId.Value);
            }

            // Thực hiện tìm kiếm theo các thuộc tính của KoiFish
            if (query.Sex.HasValue)
            {
                lots = lots.Where(l => l.KoiFish!.Sex == query.Sex.Value);
            }

            if (query.MinWeightKg.HasValue)
            {
                lots = lots.Where(l => l.KoiFish!.WeightKg >= query.MinWeightKg.Value);
            }

            if (query.MaxWeightKg.HasValue)
            {
                lots = lots.Where(l => l.KoiFish!.WeightKg <= query.MaxWeightKg.Value);
            }

            if (query.MinSizeCm.HasValue)
            {
                lots = lots.Where(l => l.KoiFish!.SizeCm >= query.MinSizeCm.Value);
            }

            if (query.MaxSizeCm.HasValue)
            {
                lots = lots.Where(l => l.KoiFish!.SizeCm <= query.MaxSizeCm.Value);
            }

            if (query.YearOfBirth.HasValue)
            {
                lots = lots.Where(l => l.KoiFish!.YearOfBirth == query.YearOfBirth.Value);
            }
            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                switch (query.SortBy.ToLower())
                {
                    case "startingprice":
                        lots = query.IsDescending
                            ? lots.OrderByDescending(l => l.StartingPrice)
                            : lots.OrderBy(l => l.StartingPrice);
                        break;

                    case "createdat":
                        lots = query.IsDescending
                            ? lots.OrderByDescending(l => l.CreatedAt)
                            : lots.OrderBy(l => l.CreatedAt);
                        break;
                    case "updatedat":
                        lots = query.IsDescending
                            ? lots.OrderByDescending(l => l.UpdatedAt)
                            : lots.OrderBy(l => l.UpdatedAt);
                        break;

                    case "sizecm":
                        lots = query.IsDescending
                            ? lots.OrderByDescending(l => l.KoiFish!.SizeCm)
                            : lots.OrderBy(l => l.KoiFish!.SizeCm);
                        break;

                    case "weightkg":
                        lots = query.IsDescending
                            ? lots.OrderByDescending(l => l.KoiFish!.WeightKg)
                            : lots.OrderBy(l => l.KoiFish!.WeightKg);
                        break;

                    default:
                        // Xử lý trường hợp không khớp với bất kỳ giá trị SortBy nào
                        break;
                }
            }

            return await lots.ToListAsync();
        }

        public async Task<Lot> GetLotByIdAsync(int id)
        {
            var lot = await _context.Lots.Include(l => l.KoiFish).ThenInclude(m => m!.KoiMedia).
                                            Include(l => l.LotStatus).
                                            Include(l => l.AuctionMethod).FirstOrDefaultAsync(l => l.LotId == id);
            if (lot == null)
                throw new KeyNotFoundException($" Lot {id} was not found");
            return lot;
        }

        public async Task<Lot> UpdateLotAsync(int id, UpdateLotDto updateLotDto)
        {
            var lot = await _context.Lots.Include(l => l.KoiFish).
                                            Include(l => l.LotStatus).
                                            Include(l => l.AuctionMethod).FirstOrDefaultAsync(l => l.LotId == id);
            if (lot == null)
                throw new KeyNotFoundException($" Lot {id} was not found");

            var koiFish = lot.KoiFish;
            lot.StartingPrice = updateLotDto.StartingPrice;
            lot.AuctionMethodId = updateLotDto.AuctionMethodId;
            await _context.AuctionMethods.FindAsync(lot.AuctionMethodId);
            await _context.LotStatuses.FindAsync(lot.LotStatusId);
            return lot;
        }

        public async Task<Lot> UpdateLotStatusAsync(int id, UpdateLotStatusDto updateLot)
        {
            var lot = await _context.Lots.Include(l => l.KoiFish).
                                            Include(l => l.LotStatus).
                                            Include(l => l.AuctionMethod).FirstOrDefaultAsync(l => l.LotId == id);
            if (lot == null)
                throw new KeyNotFoundException($" Lot {id} was not found");

            var status = await _context.LotStatuses.FirstOrDefaultAsync(x => x.LotStatusName == updateLot.LotStatusName);
            lot.LotStatusId = status!.LotStatusId;
            return lot;
        }

        //update status of a list of lots to InAuction
        public async Task<List<Lot>> UpdateLotsStatusToInAuctionAsync(List<int> lotIds)
        {
            var lots = await _context.Lots.Where(l => lotIds.Contains(l.LotId)).ToListAsync();
            if (lots.Count == 0)
                throw new KeyNotFoundException($" Lots were not found");

            // var status = await _context.LotStatuses.FirstOrDefaultAsync(x => x.LotStatusName == "In Auction");
            lots.ForEach(l => l.LotStatusId = (int)Enums.LotStatus.InAuction);
            return lots;
        }

        public async Task<List<LotAuctionMethodStatisticDto>> GetLotAuctionMethodStatisticAsync()
        {
            var totalLots = await _context.Lots.CountAsync(); //sum Lots

            var statistic = await _context.Lots
                .GroupBy(l => l.AuctionMethodId)
                .Select(g => new
                {
                    AuctionMethodId = g.Key, //why Key? 
                    Count = g.Count()
                })
                .ToListAsync();

            var auctionMethods = await _context.AuctionMethods.ToListAsync();
            //tinh toan thong ke nek
            var result = statistic.Select(s =>
            {
                var auctionMethod = auctionMethods.First(am => am.AuctionMethodId == s.AuctionMethodId);
                return new LotAuctionMethodStatisticDto
                {
                    AuctionMethodId = s.AuctionMethodId,
                    AuctionMethodName = auctionMethod.AuctionMethodName,
                    Count = s.Count,
                    Rate = totalLots > 0 ? Math.Round((double)s.Count / totalLots * 100, 2) : 0
                };
            }).ToList();

            return result;
        }

        public async Task<List<Lot>> GetBreederLotsStatisticsAsync()
        {
            var query = await _context.Lots
                .Include(l => l.LotStatus)
                .Include(l => l.AuctionLot)
                    .ThenInclude(al => al!.SoldLot)
                .ToListAsync();
            if (query.Count == 0) throw new KeyNotFoundException($" Lots was not found");
            return query;
        }

        public async Task<List<LotSearchResultDto>> GetLotSearchResults(int breederId)
        {
            var result = from lot in _context.Lots
                         join koiFish in _context.KoiFishes on lot.LotId equals koiFish.KoiFishId
                         join soldLot in _context.SoldLots on lot.LotId equals soldLot.SoldLotId into soldLotGroup
                         from soldLot in soldLotGroup.DefaultIfEmpty() // LEFT JOIN
                         where lot.BreederId == breederId && new int[] { 5, 6, 7, 8 }.Contains(lot.LotStatusId)

                         select new LotSearchResultDto
                         {
                             LotId = lot.LotId,
                             Variety = lot.KoiFish!.Variety,
                             Sex = lot.KoiFish.Sex,
                             SizeCm = lot.KoiFish.SizeCm,
                             YearOfBirth = lot.KoiFish.YearOfBirth,
                             WeightKg = lot.KoiFish.WeightKg,
                             FinalPrice = soldLot.FinalPrice > 0 ? soldLot.FinalPrice : 0,
                             Sku = lot.Sku,
                             KoiMedia = koiFish.KoiMedia != null
                                        ? koiFish.KoiMedia.Select(x => x.ToKoiMediaDtoFromKoiMedia()).ToList()
                                        : new List<KoiMediaDto>()

                         };
            return await result.ToListAsync();
        }



        public async Task<List<DailyRevenueDto>> GetStatisticsRevenue(DateTime startDateTime, DateTime endDateTime)
        {
            // Xác định khoảng thời gian 7 ngày trước đó theo offsetWeeks
            // DateTime endDateTime = DateTime.Today.AddDays(-7 * offsetWeeks); // Lùi về 7 * offsetWeeks ngày
            // endDateTime = endDateTime.AddDays(1).AddSeconds(-1); // Đến cuối ngày
            // DateTime startDateTime = endDateTime.AddDays(-6); // Lấy 6 ngày trước ngày kết thúc để có 7 ngày
            endDateTime = endDateTime.AddDays(1).AddSeconds(-1);
            // Truy vấn cơ sở dữ liệu trong khoảng thời gian đã xác định
            var lotsCompletedInRange = await (from lot in _context.Lots
                                              join soldLot in _context.SoldLots on lot.LotId equals soldLot.SoldLotId
                                              where (lot.LotStatusId == COMPLETED || lot.LotStatusId == PAYMENT_OVERDUE) &&
                                                    lot.UpdatedAt >= startDateTime &&
                                                    lot.UpdatedAt <= endDateTime
                                              select new
                                              {
                                                  lot.UpdatedAt,
                                                  soldLot.FinalPrice,
                                                  lot.LotStatusId,
                                                  lot.StartingPrice
                                              })
                                     .ToListAsync(); // Lấy dữ liệu vào bộ nhớ

            int totalDays = (endDateTime - startDateTime).Days + 1;
            // Nhóm và tính tổng doanh thu theo từng ngày
            var dailyRevenue = Enumerable.Range(0, totalDays)
                .Select(i => startDateTime.AddDays(i))
                .GroupJoin(lotsCompletedInRange,
                           date => date.Date,
                           lot => lot.UpdatedAt.Date,
                           (date, lotGroup) => new DailyRevenueDto
                           {
                               Date = date.ToString("MMM dd"), // Hiển thị ngày và tháng
                               TotalAmount = lotGroup.Sum(x =>
                           x.LotStatusId == COMPLETED
                               ? x.FinalPrice * 0.1m // Doanh thu từ lot hoàn thành
                               : x.StartingPrice * 0.2m // Doanh thu từ Payment Overdue
                       )
                           })
                .OrderBy(result => result.Date)
                .ToList();

            return dailyRevenue;
        }
    }
}