using System.ComponentModel.DataAnnotations;
using AuctionService.Dto.Lot;

namespace AuctionService.Dto.AuctionLot
{
    public class CreateAuctionLotDto
    {
        [Required]
        public TimeSpan Duration { get; set; }
        [Required]
        public int OrderInAuction { get; set; }

        public int? StepPercent { get; set; }
        [Required]
        public int AuctionLotId { get; set; }
        [Required]
        public int AuctionId { get; set; }
    }
}