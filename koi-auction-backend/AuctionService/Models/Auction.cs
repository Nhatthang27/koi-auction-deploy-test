using System;
using System.Collections.Generic;

namespace AuctionService.Models;

public partial class Auction
{
    public int AuctionId { get; set; }

    public int StaffId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string AuctionName { get; set; } = null!;

    public int AuctionStatusId { get; set; }

    public virtual ICollection<AuctionLot> AuctionLots { get; set; } = new List<AuctionLot>();

    public virtual AuctionStatus AuctionStatus { get; set; } = null!;
}
