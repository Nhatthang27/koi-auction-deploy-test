using System;
using System.Collections.Generic;

namespace AuctionService.Models;

public partial class LotStatus
{
    public int LotStatusId { get; set; }

    public string LotStatusName { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Lot> Lots { get; set; } = new List<Lot>();
}
