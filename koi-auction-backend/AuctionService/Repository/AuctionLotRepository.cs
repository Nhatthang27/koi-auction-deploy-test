using AuctionService.Data;
using AuctionService.Models;
using AuctionService.Dto.AuctionLot;
using AuctionService.Helper;
using AuctionService.IRepository;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Repository
{
    public class AuctionLotRepository : IAuctionLotRepository
    {
        private readonly AuctionManagementDbContext _context;
        public AuctionLotRepository(AuctionManagementDbContext context)
        {
            _context = context;
        }
        public async Task<AuctionLot> CreateAsync(AuctionLot auctionLot)
        {
            await _context.AddAsync(auctionLot);
            return auctionLot;
        }

        public async Task<List<AuctionLot>> CreateListAsync(List<AuctionLot> auctionLots)
        {
            await _context.AddRangeAsync(auctionLots);
            return auctionLots;
        }

        public async Task<AuctionLot> DeleteAsync(int id)
        {
            var auctionLot = await _context.AuctionLots.FirstOrDefaultAsync(a => a.AuctionLotId == id);
            if (auctionLot == null)
                throw new KeyNotFoundException($"Auction Lot {id} was not found");
            _context.Remove(auctionLot);
            return auctionLot;
        }

        public async Task<List<AuctionLot>> DeleteListAsync(List<int> ids)
        {
            var auctionLots = await _context.AuctionLots
                                .Where(a => ids.Contains(a.AuctionLotId))
                                .ToListAsync();

            if (auctionLots.Count == 0)
            {
                throw new KeyNotFoundException($"Auction Lots  was not found");
            }
            // Kiểm tra nếu tất cả các AuctionLot có trạng thái khác "Upcoming"
            var notUpcomingLots = auctionLots.Where(a => a.AuctionLotStatusId != (int)Enums.AuctionLotStatus.Upcoming).ToList();
            if (notUpcomingLots.Count == 0)
            {
                throw new Exception($"The following Auction Lots do not have an 'Upcoming' status: {string.Join(", ", notUpcomingLots.Select(al => al.AuctionLotId))}");
            }
            // Xóa các AuctionLot và lưu thay đổi vào database
            _context.AuctionLots.RemoveRange(auctionLots);
            return auctionLots; // Trả về danh sách các AuctionLot đã xóa
        }

        public async Task<List<AuctionLot>> GetAllAsync(AuctionLotQueryObject query)
        {
            var auctionLots = await _context.AuctionLots.Include(a => a.AuctionLotStatus).
                                        Include(a => a.AuctionLotNavigation)
                                            .ThenInclude(f => f.KoiFish).ThenInclude(m => m!.KoiMedia)
                                        .Include(a => a.AuctionLotNavigation)
                                            .ThenInclude(l => l.AuctionMethod)
                                        .Include(a => a.AuctionLotNavigation)
                                            .ThenInclude(s => s.LotStatus)
                                        .ToListAsync();

            if (query.AuctionId.HasValue)
            {
                auctionLots = auctionLots.Where(l => l.AuctionId == query.AuctionId.Value).ToList();
            }
            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                switch (query.SortBy.ToLower())
                {
                    case "orderinauction":
                        // Sắp xếp tăng dần OrderInAuction
                        auctionLots = auctionLots.OrderBy(l => l.OrderInAuction).ToList();
                        break;
                    default:
                        // Xử lý trường hợp không khớp với bất kỳ giá trị SortBy nào
                        break;
                }
            }
            return auctionLots;
        }

        public async Task<AuctionLot> GetAuctionLotById(int id)
        {
            var auctionLot = await _context.AuctionLots.Include(a => a.AuctionLotStatus).
            Include(a => a.AuctionLotNavigation)
                .ThenInclude(f => f.KoiFish).ThenInclude(m => m!.KoiMedia)
            .Include(a => a.AuctionLotNavigation)
                .ThenInclude(l => l.AuctionMethod)
            .Include(a => a.AuctionLotNavigation)
                .ThenInclude(s => s.LotStatus).FirstOrDefaultAsync(a => a.AuctionLotId == id);
            if (auctionLot == null)
            {
                throw new KeyNotFoundException($"Auction Lot {id} was not found");
            }
            return auctionLot;
        }

        public AuctionLot Update(AuctionLot auctionLot, UpdateAuctionLotDto updateAuctionLotDto)
        {

            if (auctionLot == null)
                throw new KeyNotFoundException($"Auction Lot was not found");
            auctionLot.Duration = updateAuctionLotDto.Duration;
            auctionLot.OrderInAuction = updateAuctionLotDto.OrderInAuction;
            auctionLot.StepPercent = updateAuctionLotDto.StepPercent;
            auctionLot.AuctionId = updateAuctionLotDto.AuctionId;
            auctionLot.StartTime = updateAuctionLotDto.StartTime;
            auctionLot.EndTime = updateAuctionLotDto.EndTime;
            auctionLot.AuctionLotStatusId = updateAuctionLotDto.AuctionLotStatusId;

            return auctionLot;
        }

        public async Task<AuctionLot?> UpdateStatusAsync(int id, int statusId)
        {
            var auctionLot = await GetAuctionLotById(id);
            if (auctionLot == null)
                throw new KeyNotFoundException($"Auction Lot {id} was not found");
            auctionLot.AuctionLotStatusId = statusId;
            return auctionLot;
        }

        public async Task<AuctionLot?> UpdateStartTimeAsync(int id, DateTime startTime)
        {
            var auctionLot = await _context.AuctionLots.FirstOrDefaultAsync(a => a.AuctionLotId == id);
            if (auctionLot == null)
                throw new KeyNotFoundException($"Auction Lot {id} was not found");
            auctionLot.StartTime = startTime;
            return auctionLot;
        }

        public async Task<AuctionLot?> GetAuctionLotByOrderInAuction(int auctionId, int orderInAuction)
        {
            var auctionLot = await _context.AuctionLots.FirstOrDefaultAsync(a => a.AuctionId == auctionId && a.OrderInAuction == orderInAuction);
            return auctionLot;
        }

        public async Task<AuctionLot?> UpdateEndTimeAsync(int id, DateTime endTime)
        {
            var auctionLot = await _context.AuctionLots.FirstOrDefaultAsync(a => a.AuctionLotId == id);
            if (auctionLot == null)
                throw new KeyNotFoundException($"Auction Lot {id} was not found");
            auctionLot.EndTime = endTime;
            return auctionLot;
        }

        public async Task<bool> IsAuctionLotInAuction(int auctionId)
        {
            var auctionLot = await _context.AuctionLots.FirstOrDefaultAsync(a => a.AuctionId == auctionId
                            && (a.AuctionLotStatusId == (int)Enums.AuctionLotStatus.Ongoing || a.AuctionLotStatusId == (int)Enums.AuctionLotStatus.Upcoming));
            return auctionLot != null;
        }

    }
}