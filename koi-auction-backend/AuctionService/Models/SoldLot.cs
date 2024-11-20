using System;
using System.Collections.Generic;

namespace AuctionService.Models;

public partial class SoldLot
{
    public int SoldLotId { get; set; }

    public int WinnerId { get; set; }

    public decimal FinalPrice { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public int BreederId { get; set; }

    public string? Address { get; set; }

    public DateTime ExpTime { get; set; }

    public virtual AuctionLot SoldLotNavigation { get; set; } = null!;
}
