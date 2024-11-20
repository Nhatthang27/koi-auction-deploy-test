using System;
using System.Collections.Generic;

namespace AuctionService.Models;

public partial class AuctionStatus
{
    public int AuctionStatusId { get; set; }

    public string AuctionStatusName { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Auction> Auctions { get; set; } = new List<Auction>();
}
