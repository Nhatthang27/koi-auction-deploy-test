using System.ComponentModel.DataAnnotations;

namespace AuctionService.Dto.LotStatus
{
    public class CreateLotStatusDto
    {
        [Required]
        public string? LotStatusName { get; set; }
    }
}