using System;
using System.Collections.Generic;

namespace AuctionService.Models;

public partial class Lot
{
    public int LotId { get; set; }

    public int BreederId { get; set; }

    public decimal StartingPrice { get; set; }

    public int LotStatusId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public int AuctionMethodId { get; set; }

    public string Sku { get; set; } = null!;

    public virtual AuctionLot? AuctionLot { get; set; }

    public virtual AuctionMethod AuctionMethod { get; set; } = null!;

    public virtual KoiFish? KoiFish { get; set; }

    public virtual LotStatus? LotStatus { get; set; }
}
