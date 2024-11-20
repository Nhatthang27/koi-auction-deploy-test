using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionService.Dto.AuctionDeposit
{
    public class AuctionDepositDto
    {
        public int AucitonLotId { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
    }
}