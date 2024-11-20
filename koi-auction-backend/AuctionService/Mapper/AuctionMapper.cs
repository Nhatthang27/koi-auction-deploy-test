using AuctionService.Models;
using AuctionService.Dto.Auction;

namespace AuctionService.Mapper
{
    public static class AuctionMapper
    {
        public static AuctionDto ToAuctionDtoFromAuction(this Auction auction)
        {
            return new AuctionDto
            {
                AuctionId = auction.AuctionId,
                AuctionName = auction.AuctionName,
                StaffId = auction.StaffId,
                AuctionStatus = auction.AuctionStatus.ToAuctionStatusDtoFromAuctionStatus(),
                StartTime = auction.StartTime,
                EndTime = auction.EndTime,
                CreatedAt = auction.CreatedAt,
            };
        }

        public static Auction ToAuctionFromCreateAuctionDto(this CreateAuctionDto createAuction, int staffId)
        {
            return new Auction
            {
                // StaffId = createAuction.StaffId,
                StaffId = staffId,
                StartTime = createAuction.StartTime,
                // EndTime = createAuction.EndTime,
            };
        }
    }
}