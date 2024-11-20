using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionService.Helper
{
    public static class TimeHelper
    {
        public static TimeSpan CalculateRemainingTime(DateTime startTime, TimeSpan duration)
        {
            return startTime.Add(duration) - DateTime.Now;
        }

        //convert date time to date time offset
        public static DateTimeOffset ConvertToDateTimeOffset(this DateTime dateTime, TimeSpan offset)
        {
            dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
            return new DateTimeOffset(dateTime, offset);
        }

        public static TimeSpan Subtract(this TimeSpan time, TimeSpan time2)
        {
            return time - time2;
        }
    }
}