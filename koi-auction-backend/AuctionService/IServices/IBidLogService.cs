using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionService.Helper;
using AuctionService.Models;

namespace AuctionService.IServices
{
    public interface IBidLogService
    {
        Task<List<BidLog>> GetAllBidLog(BidLogQueryObject queryObject);
        Task<BidLog> GetBidLogById(int id);
        //Task<BidLog> CreateBidLog(BidLog bidLog);

        Task<BidLog> GetHighestBidLogByAuctionLotId(int auctionLotId);

    }
}