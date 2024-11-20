using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AuctionService.Models;

namespace AuctionService.Dto.Lot
{
    public class UpdateLotDto
    {
        [Required]
        public decimal StartingPrice { get; set; }
        [Required]
        public int AuctionMethodId { get; set; }
    }
}