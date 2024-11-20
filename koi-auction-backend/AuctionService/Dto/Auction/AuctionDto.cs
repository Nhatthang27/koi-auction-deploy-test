using AuctionService.Models;
using AuctionService.Dto.AuctionLot;
using AuctionService.Dto.AuctionStatus;


namespace AuctionService.Dto.Auction
{
    public class AuctionDto
    {
        public int AuctionId { get; set; }
        public string? AuctionName { get; set; }
        public int StaffId { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public DateTime CreatedAt { get; set; }

        public AuctionStatusDto? AuctionStatus { get; set; }

    }
}
