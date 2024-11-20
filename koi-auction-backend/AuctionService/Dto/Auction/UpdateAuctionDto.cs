using System.ComponentModel.DataAnnotations;
using AuctionService.Dto.AuctionStatus;
using AuctionService.Helper;

namespace AuctionService.Dto.Auction
{
    public class UpdateAuctionDto
    {

        [Required]
        public int StaffId { get; set; }
        [Required]
        [CustomValidation(typeof(AuctionHelper), nameof(AuctionHelper.IsValid))]
        public DateTime StartTime { get; set; }
        [CustomValidation(typeof(AuctionHelper), nameof(AuctionHelper.IsValid))]
        public DateTime? EndTime { get; set; }
        public AuctionStatusDto? AuctionStatus { get; set; }

    }
}