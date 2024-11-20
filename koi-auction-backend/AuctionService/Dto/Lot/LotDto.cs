using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionService.Dto.AuctionMethod;
using AuctionService.Dto.BreederDetail;
using AuctionService.Dto.KoiFish;
using AuctionService.Dto.LotStatus;

namespace AuctionService.Dto.Lot
{
    public class LotDto
    {
        public int LotId { get; set; }
        public string? Sku { get; set; }
        public decimal StartingPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public AuctionMethodDto? AuctionMethod { get; set; }
        public int BreederId { get; set; }
        public KoiFishDto? KoiFishDto { get; set; }
        public LotStatusDto? LotStatusDto { get; set; }
        public BreederDetailDto? BreederDetailDto { get; set; }

    }
}