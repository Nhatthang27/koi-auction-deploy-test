using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionService.Dto.BidLog
{
    public class CreateBidLogDto
    {
        public int BidderId { get; set; }

        public int AuctionLotId { get; set; }

        public decimal BidAmount { get; set; }

    }
}