using AuctionService.Dto.AuctionDeposit;
using AuctionService.Dto.BreederDetail;
using AuctionService.Dto.KoiFish;
using AuctionService.Dto.KoiMedia;
using AuctionService.Dto.Lot;
using AuctionService.Dto.LotStatus;
using AuctionService.Dto.User;

namespace AuctionService.Dto.SoldLot
{
    public class SoldLotDto
    {
        public int SoldLotId { get; set; }
        public UserDto? WinnerDto { get; set; }
        public int WinnerId { get; set; }
        public decimal FinalPrice { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime UpdatedLot { get; set; }
        public BreederDetailDto? BreederDetailDto { get; set; }
        public int BreederId { get; set; }
        public string? Address { get; set; }
        public KoiFishDto? KoiFish { get; set; }
        public AuctionDepositDto? AuctionDepositDto { get; set; }
        public int LotStatusId { get; set; }
        public LotStatusDto? LotStatus { get; set; }
        public string? SKU { get; set; }
        // public LotDto? Lot { get; set; }
        public DateTime? ExpTime { get; set; }


    }
}