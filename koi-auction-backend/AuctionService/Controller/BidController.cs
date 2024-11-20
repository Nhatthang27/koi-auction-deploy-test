using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AuctionService.Dto.Address;
using AuctionService.Dto.AuctionDeposit;
using AuctionService.Dto.AuctionLot;
using AuctionService.Dto.Wallet;
using AuctionService.IServices;
using AuctionService.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuctionService.Controllers
{
    [ApiController]
    [Route("api/bid")]
    public class BidController : ControllerBase
    {
        private readonly BidManagementService _bidManagementService;
        private readonly WalletService _walletService;
        private readonly IAuctionDepositService auctionDepositService;

        public BidController(BidManagementService bidManagementService, WalletService walletService, IAuctionDepositService auctionDepositService)
        {
            _bidManagementService = bidManagementService;
            _walletService = walletService;
            this.auctionDepositService = auctionDepositService;
        }

        //payment
        [HttpPost("payment")]
        public async Task<IActionResult> Payment([FromBody] PaymentDto paymentDto)
        {
            if (paymentDto == null)
            {
                return BadRequest("Payment data is required");
            }
            string result = "";
            try
            {
                result = await _walletService.PaymentAsync(paymentDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(result);
        }

        [HttpPost("update-balance")]
        public async Task<IActionResult> UpdateBalance([FromBody] WalletDto walletDto)
        {
            try
            {
                var balance = await _walletService.GetBalanceByIdAsync(walletDto.BidderId);
                return Ok(balance);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("refund")]
        public async Task<IActionResult> Refund([FromBody] List<RefundDto> refundDto)
        {
            if (refundDto == null)
            {
                return BadRequest("Refund data is required");
            }
            try
            {
                var result = await _walletService.RefundAsync(refundDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("test-list-refund")]
        public async Task<IActionResult> Test([FromQuery] int auctionLotId)
        {
            System.Console.WriteLine($"enum: {Enums.AuctionDepositStatus.PendingRefund.ToString()}");
            var penRefundList = await auctionDepositService.GetAuctionDepositByStatus(auctionLotId, Enums.AuctionDepositStatus.PendingRefund.ToString());
            penRefundList.RemoveAll(a => a.UserId == 25);
            List<RefundDto> refundList = new List<RefundDto>();
            foreach (var auctionDeposit in penRefundList)
            {
                refundList.Add(new RefundDto
                {
                    UserId = auctionDeposit.UserId,
                    Amount = auctionDeposit.Amount,
                    Description = $"Refund for auction lot {auctionDeposit.AuctionLotId}"
                });
            }
            //print all refund list
            foreach (var refund in refundList)
            {
                System.Console.WriteLine($"Refund: {refund.UserId} - {refund.Amount}");
            }
            try
            {
                var result = await _walletService.RefundAsync(refundList);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("test-address")]
        public async Task<IActionResult> TestAddress()
        {
            Console.WriteLine("Test Address");
            using HttpClient httpClient = new();
            var winnerAddressResponse = await httpClient.GetAsync($"http://localhost:3000/user-service/manage/profile/address/5");
            var content = await winnerAddressResponse.Content.ReadAsStringAsync();
            var addressDto = JsonSerializer.Deserialize<AddressDto>(content);
            System.Console.WriteLine($"Address: {addressDto?.Address}");
            return Ok(addressDto);
        }
    }
}

