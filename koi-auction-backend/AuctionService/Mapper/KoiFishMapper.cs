using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionService.Models;
using AuctionService.Dto.KoiFish;
using AuctionService.Dto.LotRequestForm;

namespace AuctionService.Mapper
{
    public static class KoiFishMapper
    {
        public static KoiFish ToKoiFishFromCreateLotRequestFormDto(this CreateLotRequestFormDto lotRequestFormDto)
        {
            return new KoiFish
            {
                Variety = lotRequestFormDto.Variety,
                Sex = lotRequestFormDto.Sex,
                SizeCm = lotRequestFormDto.SizeCm,
                YearOfBirth = lotRequestFormDto.YearOfBirth,
                WeightKg = lotRequestFormDto.WeightKg
            };
        }
        public static UpdateKoiFishDto ToUpdateKoiFishDtoFromUpdateLotRequestFormDto(this UpdateLotRequestFormDto lotRequestFormDto)
        {
            return new UpdateKoiFishDto
            {
                Variety = lotRequestFormDto.Variety,
                Sex = lotRequestFormDto.Sex,
                SizeCm = lotRequestFormDto.SizeCm,
                YearOfBirth = lotRequestFormDto.YearOfBirth,
                WeightKg = lotRequestFormDto.WeightKg
            };
        }

        public static KoiFishDto ToKoiFishDtoFromKoiFish(this KoiFish koiFish)
        {
            return new KoiFishDto
            {
                Variety = koiFish.Variety,
                Sex = koiFish.Sex,
                SizeCm = koiFish.SizeCm,
                YearOfBirth = koiFish.YearOfBirth,
                WeightKg = koiFish.WeightKg,
                KoiMedia = koiFish.KoiMedia.Select(m => m.ToKoiMediaDtoFromKoiMedia()).ToList()!
            };
        }
    }
}