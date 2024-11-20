using System.ComponentModel.DataAnnotations;

namespace AuctionService.Dto.LotStatus
{
    public class UpdateStatusDto
    {
        [Required]
        public string? LotStatusName { get; set; }
    }
}