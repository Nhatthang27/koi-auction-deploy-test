using System;
using System.Collections.Generic;

namespace AuctionService.Models;

public partial class AuctionMethod
{
    public int AuctionMethodId { get; set; }

    public string AuctionMethodName { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Lot> Lots { get; set; } = new List<Lot>();
}
