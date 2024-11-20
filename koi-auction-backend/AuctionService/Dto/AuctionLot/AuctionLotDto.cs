using AuctionService.Dto.AuctionLotStatus;
using AuctionService.Dto.Lot;

namespace AuctionService.Dto.AuctionLot
{
    public class AuctionLotDto
    {
        public int AuctionId { get; set; }
        public TimeSpan Duration { get; set; }
        public int OrderInAuction { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? StepPercent { get; set; }
        public DateTime? EndTime { get; set; }
        public LotDto? LotDto { get; set; }
        public DateTime? StartTime { get; set; }
        public AuctionLotStatusDto? AuctionLotStatusDto { get; set; }
    }
}