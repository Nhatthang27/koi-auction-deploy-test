using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionService.Dto.AuctionDeposit;
using AuctionService.Dto.Mail;
using AuctionService.Dto.ScheduledTask;
using AuctionService.Dto.Wallet;
using AuctionService.IRepository;
using AuctionService.IServices;
using AuctionService.Models;

namespace AuctionService.Services
{
    public class AuctionDepositService : IAuctionDepositService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly WalletService _walletService;
        private readonly MailService _mailService;

        private readonly ITaskSchedulerService _taskSchedulerService;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public AuctionDepositService(IUnitOfWork unitOfWork, WalletService walletService, MailService mailService, ITaskSchedulerService taskSchedulerService, IServiceScopeFactory serviceScopeFactory)
        {
            _unitOfWork = unitOfWork;
            _walletService = walletService;
            _mailService = mailService;
            _taskSchedulerService = taskSchedulerService;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<AuctionDeposit> CreateAuctionDepositAsync(AuctionDeposit auctionDeposit)
        {
            System.Console.WriteLine($"service: auctionlotid: {auctionDeposit.AuctionLotId}");
            var auctionLot = await _unitOfWork.AuctionLots.GetAuctionLotById(auctionDeposit.AuctionLotId);
            if (auctionLot == null)
            {
                throw new Exception("Auction lot not found");
            }
            if (auctionLot.AuctionLotStatusId != (int)Enums.AuctionLotStatus.Scheduled)
            {
                throw new Exception("Auction lot is not scheduled");
            }
            if (await _unitOfWork.AuctionDeposits.GetAuctionDepositByAuctionLotIdAndUserId(auctionDeposit.UserId, auctionDeposit.AuctionLotId) != null)
            {
                throw new Exception("Auction deposit existed");
            }

            try
            {
                PaymentDto paymentDto = new PaymentDto()
                {
                    UserId = auctionDeposit.UserId,
                    SoldLotId = auctionDeposit.AuctionLotId,
                    Amount = auctionDeposit.Amount,
                    Description = $"Place deposit for auction lot {auctionDeposit.AuctionLotId}"
                };
                await _walletService.PaymentAsync(paymentDto);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            var newAuctionDeposit = await _unitOfWork.AuctionDeposits.CreateAuctionDepositAsync(auctionDeposit);
            await _unitOfWork.SaveChangesAsync();

            try
            {
                MailDto mailDto = new()
                {
                    UserId = auctionDeposit.UserId,
                    Subject = $"Successfully placed deposit for auction lot {auctionDeposit.AuctionLotId}",
                    Text = $"<p>Dear ,</p><p>You have successfully placed a deposit of {auctionDeposit.Amount} for auction lot {auctionDeposit.AuctionLotId}.</p><p>Thank you for your participation.</p><p>Best regards,<br/>Koi Auction SWP391 Team</p>"
                };
                await _mailService.SendMailAsync(mailDto);
                return newAuctionDeposit;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return newAuctionDeposit;
            }
        }

        public async Task<List<AuctionDeposit>> UpdateRefundedStatus(int auctionLotId, int exceptUserId)
        {
            var list = await _unitOfWork.AuctionDeposits.UpdateRefundedStatus(auctionLotId, exceptUserId);
            await _unitOfWork.SaveChangesAsync();
            return list;
        }

        public async Task<List<AuctionDeposit>> GetAuctionDepositByStatus(int auctionLotId, string status)
        {
            var list = await _unitOfWork.AuctionDeposits.GetAuctionDepositByStatus(auctionLotId, status);
            return list;
        }

        public async Task<List<AuctionDeposit>> GetAuctionDepositByAuctionLotId(int auctionLotId)
        {
            var list = await _unitOfWork.AuctionDeposits.GetAuctionDepositByAuctionLotId(auctionLotId);
            return list;
        }

        public async Task<AuctionDeposit> GetAuctionDepositByAuctionLotIdAndUserId(int userId, int auctionLotId)
        {
            var auctionDeposit = await _unitOfWork.AuctionDeposits.GetAuctionDepositByAuctionLotIdAndUserId(userId, auctionLotId);
            return auctionDeposit;
        }

        public async Task<AuctionDeposit> UpdateAuctionDepositStatusAsync(UpdateAuctionDepositDto updateAuctionDepositDto)
        {
            var auctionDeposit = await _unitOfWork.AuctionDeposits.UpdateAuctionDepositStatusAsync(updateAuctionDepositDto);
            await _unitOfWork.SaveChangesAsync();
            return auctionDeposit;
        }
    }
}