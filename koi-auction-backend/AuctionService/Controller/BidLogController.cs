using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using AuctionService.Dto.BidLog;
using AuctionService.Helper;
using AuctionService.IServices;
using AuctionService.Mapper;
using AuctionService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace AuctionService.Controllers
{
    [Route("api/bid-log")]
    [ApiController]
    public class BidLogController : ControllerBase
    {
        private readonly IBidLogService _service;


        // private readonly PlaceBidService _placeBidService;
        public BidLogController(IBidLogService service)
        {
            _service = service;
            // _placeBidService = placeBidService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllBidLog([FromQuery] BidLogQueryObject queryObject)
        {
            var bidLog = await _service.GetAllBidLog(queryObject);
            if (bidLog == null)
                return NotFound("No bid logs found.");

            var bidDtos = bidLog.Select(b => b.ToBidLogDtoFromBidLog());
            return Ok(bidDtos);
        }



        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetBidLogById([FromRoute] int id)
        {

            var bidLog = await _service.GetBidLogById(id);
            if (bidLog == null)
                return NotFound($"BidLog with ID {id} does not exist.");

            return Ok(bidLog.ToBidLogDtoFromBidLog());

        }

        [HttpGet("highest-bid/{auctionLotId:int}")]
        public async Task<IActionResult> GetHighestBidLogByAuctionLotId([FromRoute] int auctionLotId)
        {
            try
            {
                var bidLog = await _service.GetHighestBidLogByAuctionLotId(auctionLotId);
                if (bidLog == null)
                    return NotFound($"No bid logs found for auction lot with ID {auctionLotId}.");

                return Ok(bidLog.ToBidLogDtoFromBidLog());
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }

    // [HttpPost]
    // public async Task<IActionResult> PlaceBid(CreateBidLogDto placeBid)
    // {
    //     try
    //     {
    //         if (!ModelState.IsValid)
    //             return BadRequest(ModelState);

    //         var newBid = await _service.CreateBidLog(placeBid.ToBidLogFromCreateBidLogDto());
    //         var newBidDto = newBid.ToBidLogDtoFromBidLog();
    //         return CreatedAtAction(nameof(GetBidLogById), new { id = newBid.BidLogId }, newBidDto);
    //     }
    //     catch (InvalidOperationException ex)
    //     {
    //         return BadRequest(new { message = ex.Message });
    //     }
    //     catch (Exception ex)
    //     {
    //         // Log the exception (optional)
    //         return StatusCode(500, "Internal server error: " + ex.Message);
    //     }
    // }
}


