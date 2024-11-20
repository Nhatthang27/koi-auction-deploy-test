using System.ComponentModel.DataAnnotations;
using AuctionService.Dto.KoiMedia;

namespace AuctionService.Dto.Lot
{
    public class LotSearchResultDto
    {
        public int LotId { get; set; }
        public string? Variety { get; set; }
        public bool Sex { get; set; }
        public decimal SizeCm { get; set; }
        public int YearOfBirth { get; set; }
        public decimal WeightKg { get; set; }
        public List<KoiMediaDto> KoiMedia { get; set; } = new List<KoiMediaDto>();
        public string? Sku { get; set; }
        public decimal FinalPrice { get; set; }

    }
}