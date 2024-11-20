namespace AuctionService.Helper
{
    public class LotQueryObject
    {
        public int? BreederId { get; set; } = null;
        public decimal? StartingPrice { get; set; } = null;
        public int? LotStatusId { get; set; } = null;
        public int? AuctionMethodId { get; set; } = null;
        public bool? Sex { get; set; } = null;
        public int? MinSizeCm { get; set; } = null;
        public int? MaxSizeCm { get; set; } = null;
        public int? YearOfBirth { get; set; } = null;
        public decimal? MinWeightKg { get; set; } = null;
        public decimal? MaxWeightKg { get; set; } = null;
        public string? SortBy { get; set; } = null;
        public bool IsDescending { get; set; } = false;
    }
}