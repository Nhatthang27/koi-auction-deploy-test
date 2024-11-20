using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionService.Dto.AuctionLot
{
    public class UpdateEndTimeAuctionLotDto
    {
        [Required]
        public DateTime EndTime { get; set; }
    }
}