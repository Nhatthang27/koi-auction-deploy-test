using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionService.Dto.AuctionDeposit;
using AuctionService.Models;

namespace AuctionService.Mapper
{
    public static class AuctionDepositMapper
    {
        public static AuctionDeposit ToAuctionDeposit(this CreateAuctionDepositDto createAuctionDepositDto)
        {
            return new AuctionDeposit
            {
                AuctionLotId = createAuctionDepositDto.AucitonLotId,
                Amount = createAuctionDepositDto.Amount
            };
        }


        public static AuctionDepositDto ToAuctionDepositDto(this AuctionDeposit auctionDeposit)
        {
            return new AuctionDepositDto
            {
                AucitonLotId = auctionDeposit.AuctionLotId,
                UserId = auctionDeposit.UserId,
                Amount = auctionDeposit.Amount
            };
        }


    }
}