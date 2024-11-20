using AuctionService.Data;
using AuctionService.IRepository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AuctionManagementDbContext _context;

        public UnitOfWork(AuctionManagementDbContext context)
        {
            _context = context;
            AuctionLots = new AuctionLotRepository(_context);
            AuctionMethods = new AuctionMethodRepository(_context);
            Auctions = new AuctionRepository(_context);
            KoiFishes = new KoiFishRepository(_context);
            KoiMedia = new KoiMediaRepository(_context);
            Lots = new LotRepository(_context);
            LotStatuses = new LotStatusRepository(_context);
            AuctionLotStatuses = new AuctionLotStatusRepository(_context);
            AuctionStatuses = new AuctionStatusRepository(_context);
            BidLog = new BidLogRepository(_context);
            SoldLot = new SoldLotRepository(_context);
            AuctionDeposits = new AuctionDepositRepository(_context);
        }

        public IAuctionLotRepository AuctionLots { get; private set; }
        public IAuctionMethodRepository AuctionMethods { get; private set; }
        public IAuctionRepository Auctions { get; private set; }
        public IKoiFishRepository KoiFishes { get; private set; }
        public IKoiMediaRepository KoiMedia { get; private set; }
        public ILotRepository Lots { get; private set; }
        public ILotStatusRepository LotStatuses { get; private set; }
        public IAuctionStatusRepository AuctionStatuses { get; private set; }
        public IAuctionLotStatusRepository AuctionLotStatuses { get; private set; }
        public IBidLogRepository BidLog { get; private set; }
        public ISoldLotRepository SoldLot { get; private set; }
        public IAuctionDepositRepository AuctionDeposits { get; private set; }

        public async Task<bool> SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
            return true;
        }


        public void Dispose()
        {
            _context.Dispose();
        }
    }
}