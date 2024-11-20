using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionService.Dto.AuctionDeposit;
using AuctionService.Models;

namespace AuctionService.IServices
{
    public interface IAuctionDepositService
    {
        Task<List<AuctionDeposit>> GetAuctionDepositByAuctionLotId(int auctionLotId);
        Task<AuctionDeposit> CreateAuctionDepositAsync(AuctionDeposit auctionDeposit);
        Task<AuctionDeposit> UpdateAuctionDepositStatusAsync(UpdateAuctionDepositDto updateAuctionDepositDto);
        Task<AuctionDeposit> GetAuctionDepositByAuctionLotIdAndUserId(int userId, int auctionLotId);
        Task<List<AuctionDeposit>> UpdateRefundedStatus(int auctionLotId, int exceptUserId);
        Task<List<AuctionDeposit>> GetAuctionDepositByStatus(int auctionLotId, string status);
    }
}