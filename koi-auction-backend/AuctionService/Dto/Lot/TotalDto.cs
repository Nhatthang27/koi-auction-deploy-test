namespace AuctionService.Dto.Lot
{
    public class TotalDto
    {
        public int Total { get; set; }
        public int CompletedLots { get; set; }
        public int UnsoldLots { get; set; }
        public int ToShipLots { get; set; }
        public int ToReceiveLots { get; set; }
        public int CanceledSoldLots { get; set; }
        public int RejectedLots { get; set; }
    }
}