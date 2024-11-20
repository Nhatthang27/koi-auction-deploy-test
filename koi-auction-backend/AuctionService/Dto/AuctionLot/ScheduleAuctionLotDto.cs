using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionService.Dto.AuctionLot
{
    public class ScheduleAuctionLotDto
    {
        public int AuctionLotId { get; set; }

        public DateTime StartTime { get; set; }
    }
}