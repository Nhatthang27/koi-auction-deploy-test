using System;
using System.Collections.Generic;
using AuctionService.Models;

namespace AuctionService.Models;

public partial class KoiMedia
{
    public int KoiMediaId { get; set; }

    public int KoiFishId { get; set; }

    public string FilePath { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual KoiFish KoiFish { get; set; } = null!;
}
