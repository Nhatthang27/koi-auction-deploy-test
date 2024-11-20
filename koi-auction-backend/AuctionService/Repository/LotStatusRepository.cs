using AuctionService.Data;
using AuctionService.Models;
using AuctionService.Dto.AuctionLot;
using AuctionService.Dto.LotStatus;
using AuctionService.IRepository;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Repository
{
    public class LotStatusRepository : ILotStatusRepository
    {
        private readonly AuctionManagementDbContext _context;
        public LotStatusRepository(AuctionManagementDbContext context)
        {
            _context = context;
        }
        public async Task<LotStatus> CreateLotStatusAsync(LotStatus LotStatus)
        {
            await _context.AddAsync(LotStatus);

            return LotStatus;
        }

        public async Task<LotStatus> DeleteLotStatusAsync(int id)
        {
            var lotStatus = await _context.LotStatuses.FirstOrDefaultAsync(l => l.LotStatusId == id);
            if (lotStatus == null)
            {
                throw new KeyNotFoundException($"No lot status found with ID: {id}");
            }
            _context.Remove(lotStatus);

            return lotStatus;
        }

        public async Task<List<LotStatus>> GetAllAsync()
        {
            return await _context.LotStatuses.ToListAsync();
        }

        public async Task<LotStatus?> GetLotStatusByIdAsync(int id)
        {
            var status = await _context.LotStatuses.FirstOrDefaultAsync(l => l.LotStatusId == id);
            if (status == null)
            {
                throw new ArgumentException("status not existed");
            }
            return status;
        }

        // public async Task<LotStatus?> GetLotStatusByLotIdAsync(int lotId)
        // {
        //    var lot = await _context.Lots.FirstOrDefaultAsync(l => l.LotId == lotId);
        //     var status = lot.
        // }

        public async Task<LotStatus> UpdateLotStatusAsync(int id, UpdateStatusDto lotStatusDto)
        {
            var lotStatus = await _context.LotStatuses.FirstOrDefaultAsync(l => l.LotStatusId == id);
            if (lotStatus == null)
            {
                throw new KeyNotFoundException($"No lot status found with ID: {id}");
            }
            lotStatus.LotStatusName = lotStatusDto.LotStatusName!;

            return lotStatus;
        }

    }
}