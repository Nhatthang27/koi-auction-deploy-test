using System.ComponentModel.DataAnnotations;

namespace AuctionService.Dto.KoiFish
{
    public class UpdateKoiFishDto
    {
        [Required]
        public string Variety { get; set; } = null!;

        [Required]
        public bool Sex { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Size must be greater than 0")]
        public int SizeCm { get; set; }

        [Required]
        public int YearOfBirth { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Weight must be greater than 0.")]
        public decimal WeightKg { get; set; }

    }
}