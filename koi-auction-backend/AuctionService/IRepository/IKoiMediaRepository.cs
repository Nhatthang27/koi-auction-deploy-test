using AuctionService.Dto.KoiMedia;
using AuctionService.Models;

namespace AuctionService.IRepository
{
    public interface IKoiMediaRepository
    {
        Task<KoiMedia> CreateKoiMediaAsync(KoiMedia koiMedia);
        Task<List<KoiMedia>> DeleteKoiMediaAsync(int id);
    }
}