using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionService.Models;
using AuctionService.Dto.Lot;
using AuctionService.Helper;
using AuctionService.Dto.Dashboard;


namespace AuctionService.IRepository
{
        public interface ILotRepository
        {
                Task<List<Lot>> GetAllAsync(LotQueryObject lotQuery);
                Task<Lot> GetLotByIdAsync(int id);
                Task<Lot> CreateLotAsync(Lot lot);
                Task<Lot> UpdateLotAsync(int id, UpdateLotDto lotRequest);
                Task<Lot> UpdateLotStatusAsync(int id, UpdateLotStatusDto lotRequest);
                Task<Lot> DeleteLotAsync(int id);
                Task<List<LotAuctionMethodStatisticDto>> GetLotAuctionMethodStatisticAsync();
                Task<List<Lot>> GetBreederLotsStatisticsAsync();
                Task<List<LotSearchResultDto>> GetLotSearchResults(int breederId);
                // Task<List<DailyRevenueDto>> GetWeeklyRevenueOfBreeders(int year, int month, int weekOfMonth);
                Task<List<DailyRevenueDto>> GetStatisticsRevenue(DateTime startDateTime, DateTime endDateTime);
                Task<List<Lot>> UpdateLotsStatusToInAuctionAsync(List<int> lotIds);
        }
}