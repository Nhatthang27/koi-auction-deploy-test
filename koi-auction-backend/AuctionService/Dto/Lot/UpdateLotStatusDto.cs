using System.ComponentModel.DataAnnotations;

namespace AuctionService.Dto.Lot
{
    public class UpdateLotStatusDto
    {
        [Required]
        public string? LotStatusName { get; set; }
    }
}