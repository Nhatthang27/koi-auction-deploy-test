using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionService.Models;
using AuctionService.Dto.Lot;
using AuctionService.Dto.LotRequestForm;
using AuctionService.Repository;
using Microsoft.EntityFrameworkCore.Storage;

namespace AuctionService.Mapper
{
    public static class LotMapper
    {
        public static Lot ToLotFromCreateLotRequestFormDto(this CreateLotRequestFormDto lotRequestDto)
        {
            return new Lot
            {
                BreederId = lotRequestDto.BreederId,
                StartingPrice = lotRequestDto.StartingPrice,
                AuctionMethodId = lotRequestDto.AuctionMethodId
            };
        }

        public static UpdateLotDto ToUpdateLotDtoFromLotRequestFormDto(this UpdateLotRequestFormDto lotRequestDto)
        {
            return new UpdateLotDto
            {
                StartingPrice = lotRequestDto.StartingPrice,
                AuctionMethodId = lotRequestDto.AuctionMethodId
            };
        }

        public static LotDto ToLotDtoFromLot(this Lot lot)
        {
            return new LotDto
            {
                LotId = lot.LotId,
                Sku = lot.Sku,
                StartingPrice = lot.StartingPrice,
                CreatedAt = lot.CreatedAt,
                AuctionMethod = lot.AuctionMethod!.ToAuctionMethodDtoFromAuctionMethod(),
                BreederId = lot.BreederId,
                KoiFishDto = lot.KoiFish!.ToKoiFishDtoFromKoiFish(),
                LotStatusDto = lot.LotStatus!.ToLotStatusDtoFromLotStatus()
            };
        }
    }
}