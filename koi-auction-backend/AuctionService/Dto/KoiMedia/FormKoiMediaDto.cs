using System.ComponentModel.DataAnnotations;

namespace AuctionService.Dto.KoiMedia
{
    public class FormKoiMediaDto
    {
        [Required]
        public string FilePath { get; set; } = null!;

    }
}