using AuctionService.Data;
using AuctionService.IRepository;
using AuctionService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Repository
{
    public class AuctionLotStatusRepository : IAuctionLotStatusRepository
    {
        private readonly AuctionManagementDbContext _context;
        public AuctionLotStatusRepository(AuctionManagementDbContext context)
        {
            _context = context;
        }
        public async Task<List<AuctionLotStatus>> GetAllAsync()
        {
            var statuses = await _context.AuctionLotStatuses.ToListAsync();
            if (statuses == null || statuses.Count == 0)
                throw new Exception("Auction Lot Statuses not existed");
            return statuses;
        }

        public async Task<AuctionLotStatus> GetAuctionLotStatusByIdAsync(int id)
        {
            var status = await _context.AuctionLotStatuses.FirstOrDefaultAsync(a => a.AuctionLotStatusId == id);
            if (status == null)
            {
                throw new ArgumentException($"Auction Lot Status {id} not existed");
            }
            return status;
        }

    }
}