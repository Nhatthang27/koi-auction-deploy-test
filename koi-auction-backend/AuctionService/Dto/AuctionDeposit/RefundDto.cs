using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionService.Dto.AuctionDeposit
{
    public class RefundDto
    {
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public required string Description { get; set; }
    }
}