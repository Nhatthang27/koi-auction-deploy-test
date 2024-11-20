using System;
using System.Collections.Generic;

namespace AuctionService.Models;

public partial class AuctionLot
{
    public int AuctionLotId { get; set; }

    public int AuctionId { get; set; }

    public TimeSpan Duration { get; set; }

    public int OrderInAuction { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public int? StepPercent { get; set; }

    public DateTime? EndTime { get; set; }

    public DateTime? StartTime { get; set; }

    public int AuctionLotStatusId { get; set; }

    public virtual Auction Auction { get; set; } = null!;

    public virtual ICollection<AuctionDeposit> AuctionDeposits { get; set; } = new List<AuctionDeposit>();

    public virtual Lot AuctionLotNavigation { get; set; } = null!;

    public virtual AuctionLotStatus AuctionLotStatus { get; set; } = null!;

    public virtual ICollection<BidLog> BidLogs { get; set; } = new List<BidLog>();

    public virtual SoldLot? SoldLot { get; set; }
}
