using AuctionService.Models;
using AuctionService.Dto.AuctionStatus;

namespace AuctionService.Mapper
{
    public static class AuctionStatusMapper
    {
        public static AuctionStatusDto ToAuctionStatusDtoFromAuctionStatus(this AuctionStatus auctionStatus)
        {
            if (auctionStatus == null)
            {
                return null!;
            }
            return new AuctionStatusDto
            {
                AuctionStatusId = auctionStatus.AuctionStatusId,
                AuctionStatusName = auctionStatus.AuctionStatusName
            };
        }
    }
}