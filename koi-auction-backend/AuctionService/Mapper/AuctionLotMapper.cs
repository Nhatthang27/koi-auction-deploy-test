using AuctionService.Models;
using AuctionService.Dto.AuctionLot;

namespace AuctionService.Mapper
{
    public static class AuctionLotMapper
    {
        public static AuctionLotDto ToAuctionLotDtoFromAuctionLot(this AuctionLot auctionLot)
        {
            if (auctionLot == null)
            {
                return null!;
            }

            return new AuctionLotDto
            {
                AuctionId = auctionLot.AuctionId,
                Duration = auctionLot.Duration,
                OrderInAuction = auctionLot.OrderInAuction,
                CreatedAt = auctionLot.CreatedAt,
                StepPercent = auctionLot.StepPercent,
                EndTime = auctionLot.EndTime,
                StartTime = auctionLot.StartTime,
                LotDto = auctionLot.AuctionLotNavigation.ToLotDtoFromLot(),
                AuctionLotStatusDto = auctionLot.AuctionLotStatus.ToAuctionLotStatusDtoFromAuctionLotStatus()
            };
        }

        public static UpdateAuctionLotDto ToUpdateAuctionLotDtoFromAuctionLot(this AuctionLot auctionLot)
        {
            if (auctionLot == null)
            {
                return null!;
            }

            return new UpdateAuctionLotDto
            {
                AuctionId = auctionLot.AuctionId,
                Duration = auctionLot.Duration,
                OrderInAuction = auctionLot.OrderInAuction,
                StepPercent = auctionLot.StepPercent,
                EndTime = auctionLot.EndTime,
                StartTime = auctionLot.StartTime,
                AuctionLotStatusId = auctionLot.AuctionLotStatusId
            };
        }

        public static AuctionLot ToAuctionLotFromCreateAuctionLotDto(this CreateAuctionLotDto auctionLotDto)
        {
            return new AuctionLot
            {
                Duration = auctionLotDto.Duration,
                OrderInAuction = auctionLotDto.OrderInAuction,
                StepPercent = auctionLotDto.StepPercent,
                AuctionLotId = auctionLotDto.AuctionLotId,
                AuctionId = auctionLotDto.AuctionId
            };
        }

        public static AuctionLotBidDto ToAuctionLotBidDtoFromAuctionLot(this AuctionLot auctionLot)
        {
            if (auctionLot == null)
            {
                return null!;
            }

            return new AuctionLotBidDto
            {
                AuctionLotId = auctionLot.AuctionLotId,
                AuctionMethodId = auctionLot.AuctionLotNavigation.AuctionMethodId,
                StartPrice = auctionLot.AuctionLotNavigation.StartingPrice,
                StepPercent = auctionLot.StepPercent,
                Duration = auctionLot.Duration,
                RemainingTime = auctionLot.Duration,
                StartTime = auctionLot.StartTime,
                PredictEndTime = auctionLot.StartTime + auctionLot.Duration
            };
        }
    }
}