namespace AuctionService.Dto.Lot
{
  public class LotAuctionMethodStatisticDto
  {
    public int AuctionMethodId { get; set; }
    public string? AuctionMethodName { get; set; }
    public int Count { get; set; }
    public double Rate { get; set; }
  }
}
