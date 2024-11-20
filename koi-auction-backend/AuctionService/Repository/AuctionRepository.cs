using AuctionService.Data;
using AuctionService.Models;
using AuctionService.Dto.Auction;
using AuctionService.Helper;
using AuctionService.IRepository;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Repository
{
    public class AuctionRepository : IAuctionRepository
    {
        private readonly AuctionManagementDbContext _context;

        public AuctionRepository(AuctionManagementDbContext context)
        {
            _context = context;
        }
        public async Task<Auction> CreateAsync(Auction auction)
        {
            await _context.AddAsync(auction);

            return auction;
        }

        public async Task<Auction> DeleteAsync(int id)
        {
            var auction = await _context.Auctions.FirstOrDefaultAsync(a => a.AuctionId == id);
            if (auction == null)
                throw new KeyNotFoundException($"Auction {id} was not found");
            _context.Remove(auction!);

            return auction!;
        }

        public async Task<List<Auction>> GetAllAsync(AuctionQueryObject query)
        {
            var auctions = _context.Auctions.Include(a => a.AuctionLots)
                                                .ThenInclude(l => l.AuctionLotNavigation)
                                                    .ThenInclude(k => k.KoiFish)
                                            .Include(a => a.AuctionLots)
                                                .ThenInclude(a => a.AuctionLotNavigation)
                                                    .ThenInclude(l => l.AuctionMethod)
                                            .Include(a => a.AuctionStatus)
                                            .Include(a => a.AuctionLots)
                                                .ThenInclude(a => a.AuctionLotNavigation)
                                                    .ThenInclude(s => s.LotStatus).Include(a => a.AuctionStatus).AsQueryable();

            // Lọc theo StaffId nếu có
            if (query.StaffId.HasValue)
            {
                auctions = auctions.Where(a => a.StaffId == query.StaffId.Value);
            }

            // Lọc theo StartTime nếu có
            if (query.StartTime.HasValue)
            {
                auctions = auctions.Where(a => a.StartTime.Date == query.StartTime.Value.Date);
            }

            // Lọc theo CreatedAt nếu có
            if (query.CreatedAt.HasValue)
            {
                auctions = auctions.Where(a => a.CreatedAt.Date == query.CreatedAt.Value.Date);
            }
            return await auctions.ToListAsync();
        }

        public async Task<Auction> GetByIdAsync(int id)
        {
            var auction = await _context.Auctions.Include(a => a.AuctionLots)
                                                .ThenInclude(l => l.AuctionLotNavigation)
                                                    .ThenInclude(k => k.KoiFish)
                                            .Include(a => a.AuctionLots)
                                                .ThenInclude(a => a.AuctionLotNavigation)
                                                    .ThenInclude(l => l.AuctionMethod)
                                            .Include(a => a.AuctionStatus)
                                            .Include(a => a.AuctionLots)
                                                .ThenInclude(a => a.AuctionLotNavigation)
                                                    .ThenInclude(s => s.LotStatus).Include(a => a.AuctionStatus).FirstOrDefaultAsync(a => a.AuctionId == id);
            if (auction == null)
                throw new KeyNotFoundException($"Auction {id} was not found");
            return auction;
        }

        public async Task<Auction> UpdateAsync(int id, UpdateAuctionDto updateAuctionDto)
        {
            var auction = await _context.Auctions.FirstOrDefaultAsync(a => a.AuctionId == id);
            if (auction == null)
                throw new KeyNotFoundException($"Auction {id} was not found");
            auction!.StaffId = updateAuctionDto.StaffId;
            auction.StartTime = updateAuctionDto.StartTime;
            auction.EndTime = updateAuctionDto.EndTime;

            return auction;
        }

        public async Task<Auction?> UpdateStatusAsync(int id, int auctionStatusId)
        {
            var auction = await _context.Auctions.FirstOrDefaultAsync(a => a.AuctionId == id);
            if (auction == null)
                throw new KeyNotFoundException($"Auction {id} was not found");

            auction.AuctionStatusId = auctionStatusId;

            return auction;
        }

        public async Task<Auction?> UpdateEndTimeAsync(int id, DateTime endTime)
        {
            var auction = await _context.Auctions.FirstOrDefaultAsync(a => a.AuctionId == id);
            if (auction == null)
                throw new KeyNotFoundException($"Auction {id} was not found");
            auction.EndTime = endTime;

            return auction;
        }

    }
}