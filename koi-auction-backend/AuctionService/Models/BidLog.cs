using System;
using System.Collections.Generic;

namespace AuctionService.Models;

public partial class BidLog
{
    public int BidLogId { get; set; }

    public int BidderId { get; set; }

    public int AuctionLotId { get; set; }

    public decimal BidAmount { get; set; }

    public DateTime BidTime { get; set; }

    public virtual AuctionLot AuctionLot { get; set; } = null!;
}
