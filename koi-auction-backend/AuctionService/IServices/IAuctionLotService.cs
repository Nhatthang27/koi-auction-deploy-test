using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionService.Dto.AuctionLot;
using AuctionService.Helper;
using AuctionService.Models;

namespace AuctionService.IServices
{
    public interface IAuctionLotService
    {
        public Task ScheduleAuctionLotAsync(int auctionLotId, DateTime startTime);
        public Task StartAuctionLotAsync(int auctionLotId);
        Task<AuctionLot> DeleteAsync(int id);
        Task<bool> DeleteListAsync(List<int> ids);
        Task<AuctionLot> CreateAsync(CreateAuctionLotDto auctionLot);
        Task<List<AuctionLot>> CreateListAsync(List<CreateAuctionLotDto> auctionLots);
        Task<List<AuctionLotBidDto>> SearchAuctionLot(AuctionLotQueryObject queryObject);

    }
}