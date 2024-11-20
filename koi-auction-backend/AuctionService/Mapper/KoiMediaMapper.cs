using System.Runtime.CompilerServices;
using AuctionService.Dto.KoiMedia;
using AuctionService.Dto.LotRequestForm;
using AuctionService.Models;

namespace AuctionService.Mapper
{
    public static class KoiMediaMapper
    {
        public static KoiMedia ToKoiMediaFromFormKoiMediaDto(this FormKoiMediaDto koiMediaDto)
        {
            return new KoiMedia
            {
                FilePath = koiMediaDto.FilePath
            };
        }

        public static KoiMediaDto ToKoiMediaDtoFromKoiMedia(this KoiMedia koiMedia)
        {
            return new KoiMediaDto
            {
                KoiMediaId = koiMedia.KoiMediaId,
                KoiFishId = koiMedia.KoiFishId,
                FilePath = koiMedia.FilePath
            };
        }

        // public static CreateKoiMediaDto ToCreateKoiMediaDtoFromKoiMedia(this KoiMedia koiMedia)
        // {
        //     return new CreateKoiMediaDto
        //     {

        //         KoiFishId = koiMedia.KoiFishId,
        //         FilePath = koiMedia.FilePath,
        //         IsPrimary = koiMedia.IsPrimary
        //     };
        // }
    }
}