using AuctionService.Models;

namespace AuctionService.Helper
{
    public static class LotHelper
    {
        public static string GenerateSku(Lot lot)
        {
            string breederPart = $"BRD{lot.BreederId}";

            string hourPart = DateTime.UtcNow.Hour.ToString("D2");   // Giờ (2 chữ số)
            string minutePart = DateTime.UtcNow.Minute.ToString("D2"); // Phút (2 chữ số)
            string secondPart = DateTime.UtcNow.Second.ToString("D2");

            string dayPart = DateTime.UtcNow.Day.ToString("D2");    // Ngày (2 chữ số)
            string monthPart = DateTime.UtcNow.Month.ToString("D2"); // Tháng (2 chữ số)
            string yearPart = DateTime.UtcNow.Year.ToString();

            return $"{breederPart}-{hourPart}{minutePart}{secondPart}-{dayPart}{monthPart}{yearPart}";
        }
    }
}