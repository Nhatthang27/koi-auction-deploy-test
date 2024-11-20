using System.ComponentModel.DataAnnotations;

namespace AuctionService.Helper
{
    public class KoiFishHelper
    {
        public static ValidationResult? IsValid(int yearOfBirth, ValidationContext validationContext)
        {
            var curYear = DateTime.Now.Year;
            if ((yearOfBirth > curYear))
            {
                throw new Exception("Year of birth cannot be greater than the current year.");
            }
            return ValidationResult.Success;
        }
    }
}