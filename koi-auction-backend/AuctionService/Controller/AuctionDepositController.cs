using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AuctionService.Models;
using AuctionService.IServices;
using AuctionService.Dto.AuctionDeposit;
using AuctionService.Mapper;
using AuctionService.Dto.Mail;

namespace AuctionService.Controller
{
    [ApiController]
    [Route("api/auction-deposit")]
    public class AuctionDepositController : ControllerBase
    {
        private readonly IAuctionDepositService _auctionDepositService;

        public AuctionDepositController(IAuctionDepositService auctionDepositService)
        {
            _auctionDepositService = auctionDepositService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAuctionDepositAsync([FromBody] CreateAuctionDepositDto createAuctionDepositDto)
        {
            try
            {
                //get userID from header
                var userId = HttpContext.Request.Headers["uid"].FirstOrDefault();
                if (userId == null)
                {
                    return BadRequest("User id is required");
                }
                System.Console.WriteLine($"controller: auctionLotID: {createAuctionDepositDto.AucitonLotId}");
                var auctionDeposit = new AuctionDeposit()
                {
                    UserId = int.Parse(userId),
                    AuctionLotId = createAuctionDepositDto.AucitonLotId,
                    Amount = createAuctionDepositDto.Amount,
                    AuctionDepositStatus = Enums.AuctionDepositStatus.PendingRefund
                };
                var result = await _auctionDepositService.CreateAuctionDepositAsync(auctionDeposit);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetAuctionDepositAsync([FromQuery] int auctionLotId, [FromQuery] int userId)
        {
            try
            {
                var result = await _auctionDepositService.GetAuctionDepositByAuctionLotIdAndUserId(userId, auctionLotId);
                if (result == null)
                {
                    return NotFound();
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}