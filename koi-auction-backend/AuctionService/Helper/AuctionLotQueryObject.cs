using AuctionService.Dto.AuctionLotStatus;
using AuctionService.Dto.BreederDetail;
using AuctionService.Dto.LotStatus;

namespace AuctionService.Helper
{
    public class AuctionLotQueryObject
    {
        public int? AuctionId { get; set; } = null;
        public string? SortBy { get; set; } = null;


        // //search
        // public int? AuctionMethodId { get; set; } = null;
        // public bool? Sex { get; set; } = null;
        // public int? YearOfBirth { get; set; } = null;
        // public decimal? FinalPrice { get; set; } = null;
        // public decimal? StartingPrice { get; set; } = null;
        // public LotStatusDto? LotStatusDto { get; set; } = null;
        // public AuctionLotStatusDto? AuctionLotStatusDto { get; set; } = null;

    }
}