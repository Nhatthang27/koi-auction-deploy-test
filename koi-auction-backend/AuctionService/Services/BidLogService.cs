using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionService.Dto.BidLog;
using AuctionService.Helper;
using AuctionService.IRepository;
using AuctionService.IServices;
using AuctionService.Mapper;
using AuctionService.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Services
{
    public class BidLogService : IBidLogService
    {
        private readonly IUnitOfWork _unitOfWork;
        public BidLogService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //call repository to get highest bid log by auction lot id
        public async Task<BidLog> GetHighestBidLogByAuctionLotId(int auctionLotId)
        {

            var bid = await _unitOfWork.BidLog.GetHighestBidLogByAuctionLotId(auctionLotId);
            return bid!;
        }

        // public async Task<BidLog> CreateBidLog(BidLog bidLog)
        // {
        //     try
        //     {
        //         var curBid = await _unitOfWork.BidLog.GetMaxAmountByAuctionLotIdAsync(bidLog.AuctionLotId);

        //         if (curBid != null)
        //         {
        //             if (bidLog.BidAmount <= curBid.MaxBidAmount)
        //             {
        //                 throw new InvalidOperationException("Bid amount must be higher than the current highest bid.");
        //             }
        //         }

        //         var newBid = await _unitOfWork.BidLog.CreateAsync(bidLog);
        //         if (!await _unitOfWork.SaveChangesAsync())
        //         {
        //             throw new Exception("An error occurred while saving the data");
        //         }
        //         return newBid;
        //     }
        //     catch (InvalidOperationException ex)
        //     {
        //         // Xử lý ngoại lệ liên quan đến giá thầu không hợp lệ
        //         throw new InvalidOperationException(ex.Message, ex);
        //     }
        //     catch (DbUpdateException ex)
        //     {
        //         // Xử lý lỗi khi cập nhật cơ sở dữ liệu
        //         throw new Exception("An error occurred while saving the bid log. Please try again.", ex);
        //     }
        //     catch (Exception ex)
        //     {
        //         // Xử lý các ngoại lệ không mong đợi
        //         throw new Exception("An unexpected error occurred while creating the bid log.", ex);
        //     }
        // }

        public async Task<List<BidLog>> GetAllBidLog(BidLogQueryObject queryObject)
        {
            return await _unitOfWork.BidLog.GetAllAsync(queryObject);
        }

        public async Task<BidLog> GetBidLogById(int id)
        {
            var bidLog = await _unitOfWork.BidLog.GetByIdAsync(id);
            return bidLog;
        }
    }
}