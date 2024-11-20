using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionService.Dto.BidLog;
using AuctionService.Helper;
using AuctionService.Models;

namespace AuctionService.IRepository
{
    public interface IBidLogRepository
    {
        Task<List<BidLog>> GetAllAsync(BidLogQueryObject queryObject);
        Task<BidLog> GetByIdAsync(int id);
        Task<BidLog> CreateAsync(BidLog bidLog);

        Task<BidLog?> GetHighestBidLogByAuctionLotId(int auctionLotId);
    }

}