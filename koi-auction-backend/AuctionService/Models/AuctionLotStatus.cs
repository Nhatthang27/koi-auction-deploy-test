using System;
using System.Collections.Generic;

namespace AuctionService.Models;

public partial class AuctionLotStatus
{
    public int AuctionLotStatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<AuctionLot> AuctionLots { get; set; } = new List<AuctionLot>();
}
