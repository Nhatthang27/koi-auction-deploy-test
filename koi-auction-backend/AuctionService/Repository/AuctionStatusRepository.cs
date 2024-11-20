using AuctionService.Data;
using AuctionService.Models;
using AuctionService.IRepository;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Repository
{
    public class AuctionStatusRepository : IAuctionStatusRepository
    {
        private readonly AuctionManagementDbContext _context;
        public AuctionStatusRepository(AuctionManagementDbContext context)
        {
            _context = context;
        }


        public async Task<List<AuctionStatus>> GetAllAsync()
        {
            return await _context.AuctionStatuses.ToListAsync();
        }

        public async Task<AuctionStatus> GetAuctionStatusByIdAsync(int id)
        {
            var status = await _context.AuctionStatuses.FirstOrDefaultAsync(l => l.AuctionStatusId == id);
            if (status == null)
            {
                throw new ArgumentException("status not existed");
            }
            return status;
        }



    }
}