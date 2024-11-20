using System.ComponentModel.DataAnnotations;
using AuctionService.Dto.KoiMedia;

namespace AuctionService.Dto.LotRequestForm
{
    public class UpdateLotRequestFormDto
    {
        // update lot
        [Required]
        public int LotStatusId { get; set; }
        [Required]
        [Range(50.0, double.MaxValue, ErrorMessage = "StartPrice must be greater than 50")]
        public decimal StartingPrice { get; set; }
        [Required]
        public int AuctionMethodId { get; set; }
        [Required]

        //update koifish
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

        //update media
        [Required]
        public List<FormKoiMediaDto> KoiMedia { get; set; } = new List<FormKoiMediaDto>();
    }
}