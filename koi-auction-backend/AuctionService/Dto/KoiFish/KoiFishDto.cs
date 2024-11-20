using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionService.Dto.KoiMedia;

namespace AuctionService.Dto.KoiFish
{
    public class KoiFishDto
    {
        public string Variety { get; set; } = null!;

        public bool Sex { get; set; }

        public decimal SizeCm { get; set; }

        public int YearOfBirth { get; set; }

        public decimal WeightKg { get; set; }

        public List<KoiMediaDto?> KoiMedia { get; set; } = new List<KoiMediaDto?>();
    }
}