using AuctionService.Data;
using AuctionService.Dto.KoiMedia;
using AuctionService.IRepository;
using AuctionService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Repository
{
    public class KoiMediaRepository : IKoiMediaRepository
    {
        private readonly AuctionManagementDbContext _context;
        public KoiMediaRepository(AuctionManagementDbContext context)
        {
            _context = context;
        }
        public async Task<KoiMedia> CreateKoiMediaAsync(KoiMedia koiMedia)
        {
            await _context.KoiMedia.AddAsync(koiMedia);

            return koiMedia;
        }

        public async Task<List<KoiMedia>> DeleteKoiMediaAsync(int id)
        {
            var deleteKoiMedia = await _context.KoiMedia.Where(m => m.KoiFishId == id).ToListAsync();
            if (deleteKoiMedia == null)
            {
                throw new KeyNotFoundException($"No koi media found with koi fish ID: {id}");
            }
            foreach (var media in deleteKoiMedia)
            {
                _context.KoiMedia.Remove(media);
            }
            return deleteKoiMedia;
        }
    }
}