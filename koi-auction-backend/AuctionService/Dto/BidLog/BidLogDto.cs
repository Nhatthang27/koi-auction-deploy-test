using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionService.Dto.BidLog
{
    public class BidLogDto
    {
        public int BidLogId { get; set; }

        public int BidderId { get; set; }

        public int AuctionLotId { get; set; }

        public decimal BidAmount { get; set; }

        public DateTime BidTime { get; set; }
    }
}