using System.ComponentModel.DataAnnotations;

namespace AuctionService.Dto
{
    public class UpdateAuctionMethodDto
    {
        [Required]
        public string? AuctionMethodName { get; set; }
        [Required]
        public string? Description { get; set; }
    }
}