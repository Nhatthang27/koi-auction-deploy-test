using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionService.Enums
{
    public static class AuctionDepositStatus
    {
        public const string PendingRefund = "Pending Refund";
        public const string Refunded = "Refunded";
        public const string DepositApplied = "Deposit Applied";
        public const string DepositForfeiture = "Deposit Forfeiture";

    }

}