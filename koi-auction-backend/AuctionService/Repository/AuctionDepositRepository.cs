using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionService.Data;
using AuctionService.Dto.AuctionDeposit;
using AuctionService.IRepository;
using AuctionService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Repository
{
    public class AuctionDepositRepository : IAuctionDepositRepository
    {
        private readonly AuctionManagementDbContext _context;
        public AuctionDepositRepository(AuctionManagementDbContext context)
        {
            _context = context;
        }
        public async Task<AuctionDeposit> CreateAuctionDepositAsync(AuctionDeposit auctionDeposit)
        {
            await _context.AddAsync(auctionDeposit);
            return auctionDeposit;
        }

        public async Task<List<AuctionDeposit>> GetAuctionDepositByAuctionLotId(int auctionLotId)
        {
            var list = _context.AuctionDeposits.Where(a => a.AuctionLotId == auctionLotId);
            if (list == null || list.Count() == 0)
            {
                // throw new ArgumentException($"Auction Deposit {auctionLotId} not existed");
                return null!;
            }
            return await list.ToListAsync();
        }

        public async Task<AuctionDeposit> GetAuctionDepositByAuctionLotIdAndUserId(int userId, int auctionLotId)
        {
            var auctionDeposit = await _context.AuctionDeposits.FirstOrDefaultAsync(a => a.AuctionLotId == auctionLotId && a.UserId == userId);
            if (auctionDeposit == null)
            {
                // throw new ArgumentException($"Auction Deposit in {auctionLotId} by {userId} not existed");
                return null!;
            }
            return auctionDeposit;
        }

        public async Task<AuctionDeposit> UpdateAuctionDepositStatusAsync(UpdateAuctionDepositDto updateAuctionDepositDto)
        {
            var curAuctionDeposit = await _context.AuctionDeposits.FirstOrDefaultAsync(a => a.AuctionLotId == updateAuctionDepositDto.AucitonLotId && a.UserId == updateAuctionDepositDto.UserId);
            if (curAuctionDeposit == null)
            {
                throw new ArgumentException($"Auction Deposit {updateAuctionDepositDto.AucitonLotId} not existed");
            }
            curAuctionDeposit.AuctionDepositStatus = updateAuctionDepositDto.Status;
            return curAuctionDeposit;
        }

        public async Task<List<AuctionDeposit>> UpdateRefundedStatus(int auctionLotId, int exceptUserId)
        {
            var refundedList = await _context.AuctionDeposits.Where(a => a.AuctionLotId == auctionLotId &&
                                                                    a.UserId != exceptUserId &&
                                                                    a.AuctionDepositStatus == Enums.AuctionDepositStatus.PendingRefund).ToListAsync();
            refundedList.ForEach(a => a.AuctionDepositStatus = Enums.AuctionDepositStatus.Refunded);
            return refundedList;
        }

        public async Task<List<AuctionDeposit>> GetAuctionDepositByStatus(int auctionLotId, string status)
        {
            var list = await _context.AuctionDeposits.Where(a => a.AuctionLotId == auctionLotId && a.AuctionDepositStatus == status).ToListAsync();
            return list;
        }
    }
}