namespace AuctionService.Dto.SoldLot
{
    public class CreateSoldLotDto
    {
        public int SoldLotId { get; set; }

        public int WinnerId { get; set; }

        public decimal FinalPrice { get; set; }
    }
}