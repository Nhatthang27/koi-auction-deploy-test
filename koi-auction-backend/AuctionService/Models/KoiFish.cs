using System;
using System.Collections.Generic;

namespace AuctionService.Models;

public partial class KoiFish
{
    public int KoiFishId { get; set; }

    public string Variety { get; set; } = null!;

    public bool Sex { get; set; }

    public decimal SizeCm { get; set; }

    public int YearOfBirth { get; set; }

    public decimal WeightKg { get; set; }

    public virtual Lot KoiFishNavigation { get; set; } = null!;

    public virtual ICollection<KoiMedia> KoiMedia { get; set; } = new List<KoiMedia>();
}
