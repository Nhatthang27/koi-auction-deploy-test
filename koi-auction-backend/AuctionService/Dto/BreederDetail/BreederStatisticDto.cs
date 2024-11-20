public class BreederStatisticDto
{
    public int BreederId { get; set; }
    public string FarmName { get; set; } = string.Empty;
    public int TotalAuctionLot { get; set; }
    public int CountSuccess { get; set; }
    public int CountUnsuccess { get; set; }
    public double PercentUnsold { get; set; }
    public double PercentCancelledSoldLot { get; set; }
    public double PercentSuccess { get; set; }
    public double PercentUnsuccess { get; set; }
    public int Priority { get; set; }
}