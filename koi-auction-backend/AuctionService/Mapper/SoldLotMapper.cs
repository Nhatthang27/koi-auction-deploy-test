using AuctionService.Dto.SoldLot;
using AuctionService.Models;

namespace AuctionService.Mapper
{
    public static class SoldLotMapper
    {
        public static SoldLot ToSoldLotDtoFromCreateSoldLotDto(this CreateSoldLotDto createSoldLotDto)
        {
            return new SoldLot
            {
                SoldLotId = createSoldLotDto.SoldLotId,
                WinnerId = createSoldLotDto.WinnerId,
                FinalPrice = createSoldLotDto.FinalPrice
            };
        }

        public static SoldLotDto ToSoldLotDtoFromSoldLot(this SoldLot soldLot)
        {
            return new SoldLotDto
            {
                SoldLotId = soldLot.SoldLotId,
                WinnerId = soldLot.WinnerId,
                FinalPrice = soldLot.FinalPrice,
                CreatedAt = soldLot.CreatedAt,
                UpdatedAt = soldLot.UpdatedAt,
                UpdatedLot = soldLot.SoldLotNavigation.AuctionLotNavigation.UpdatedAt,
                BreederId = soldLot.BreederId,
                Address = soldLot.Address,
                KoiFish = soldLot.SoldLotNavigation.AuctionLotNavigation.KoiFish!.ToKoiFishDtoFromKoiFish(),
                LotStatusId = soldLot.SoldLotNavigation.AuctionLotNavigation.LotStatusId,
                SKU = soldLot.SoldLotNavigation.AuctionLotNavigation.Sku,
                // Lot = soldLot.SoldLotNavigation.AuctionLotNavigation.ToLotDtoFromLot()
            };
        }
    }
}