using AuctionService.Data;
using AuctionService.Dto.SoldLot;
using AuctionService.Helper;
using AuctionService.IRepository;
using AuctionService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Repository
{
    public class SoldLotRepository : ISoldLotRepository
    {
        private readonly AuctionManagementDbContext _context;
        public SoldLotRepository(AuctionManagementDbContext context)
        {
            _context = context;
        }
        public async Task<SoldLot> CreateSoldLot(SoldLot soldLot)
        {
            await _context.AddAsync(soldLot);
            return soldLot;
        }

        public async Task<List<SoldLot>> GetAllAsync(SoldLotQueryObject query)
        {
            var soldLots = _context.SoldLots.Include(x => x.SoldLotNavigation).
                                                ThenInclude(x => x.AuctionLotNavigation).
                                                ThenInclude(x => x.KoiFish).
                                                ThenInclude(x => x!.KoiMedia).AsQueryable();
            if (soldLots == null)
                throw new Exception("SoldLots is null");

            if (query.UserID.HasValue)
                soldLots = soldLots.Where(x => x.WinnerId == query.UserID.Value);
            if (query.BreederID.HasValue)
                soldLots = soldLots.Where(x => x.BreederId == query.BreederID.Value);

            //1. lot query -> lot status id
            //2. soldLot == lot ON lotId
            //3. soldLot -> lot -> lotStatusId
            
            var newSoldLots = new List<SoldLot>();
            if (query.LotStatusID.HasValue) {
                foreach (var soldLot in soldLots)
                {
                    var lotStatusId = soldLot.SoldLotNavigation.AuctionLotNavigation.LotStatusId;
                    if (lotStatusId == query.LotStatusID.Value)
                    {
                        newSoldLots.Add(soldLot);
                    }
                }
            } else {
                return await soldLots.ToListAsync();
            }

            return await Task.FromResult(newSoldLots);
        }

        public async Task<SoldLot> GetSoldLotById(int id)
        {
            var soldLot = await _context.SoldLots.Include(x => x.SoldLotNavigation).
                                                ThenInclude(x => x.AuctionLotNavigation).
                                                ThenInclude(x => x.KoiFish).
                                                ThenInclude(x => x!.KoiMedia).FirstOrDefaultAsync(s => s.SoldLotId == id);
            if (soldLot == null)
            {
                throw new Exception($"Sold Lot is not existed with ID: {id}");
            }
            return soldLot;
        }

    }
}