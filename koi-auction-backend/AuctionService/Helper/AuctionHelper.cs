using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using AuctionService.Models;

namespace AuctionService.Helper
{
    public static class AuctionHelper
    {
        public static string GenerateAuctionName(Auction auction)
        {
            // string staffPart = $"STF{auction.StaffId}";
            // string startTime = auction.StartTime.ToString("ddHHmmss"); ;    // Ngày (2 chữ số)
            // string dayPart = DateTime.UtcNow.Day.ToString("D2"); // Tháng (2 chữ số)
            string datePart = auction.StartTime.ToString("dd/MM/yyyy");
            string timestamp = auction.StartTime.ToString("HHmmss");  // Thời gian theo định dạng YYYYMMDDHHMMSS
            return $"AUCTION-{datePart} #{timestamp}";
            // return $"AUCTION-{staffPart}#{startTime}{dayPart}";
        }

        public static ValidationResult? ValidateFutureDate(DateTime startTime, ValidationContext context)
        {
            if (startTime <= DateTime.Now)
            {
                return new ValidationResult("Start time must be in the future.");
            }

            return ValidationResult.Success;
        }

        public static ValidationResult? IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("The start time is required.");
            }
            if (!(value is DateTime))
            {
                return new ValidationResult("The start time must be in a valid date format.");
            }
            var startTime = (DateTime)value;
            if (startTime <= DateTime.Now)
            {
                return new ValidationResult("The start time must be in the future.");
            }
            return ValidationResult.Success;
        }

    }
}