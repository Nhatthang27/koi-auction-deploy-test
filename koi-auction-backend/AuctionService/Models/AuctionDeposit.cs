using System;
using System.Collections.Generic;

namespace AuctionService.Models;

public partial class AuctionDeposit
{
    public int AuctionDepositId { get; set; }

    public int UserId { get; set; }

    public int AuctionLotId { get; set; }

    public decimal Amount { get; set; }

    public string AuctionDepositStatus { get; set; } = null!;

    public DateTimeOffset? CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }

    public virtual AuctionLot AuctionLot { get; set; } = null!;
}
