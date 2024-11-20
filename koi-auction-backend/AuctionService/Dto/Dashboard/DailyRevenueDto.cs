namespace AuctionService.Dto.Dashboard
{
    public class DailyRevenueDto
    {
        public string? Date { get; set; } // Chỉ cần tên ngày (VD: "Nov 03")
        public decimal TotalAmount { get; set; } // Tổng doanh thu cho ngày đó
    }
}