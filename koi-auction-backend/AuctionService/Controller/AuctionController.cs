using AuctionService.Dto.Auction;
using AuctionService.Helper;
using AuctionService.IRepository;
using AuctionService.IServices;
using AuctionService.Mapper;
using AuctionService.Models;
using AuctionService.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuctionService.Controller
{
    [Route("api/auctions")]
    [ApiController]
    public class AuctionController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        // private readonly BreederDetailService _breederService;
        private readonly IAuctionService _auctionService;

        public AuctionController(IUnitOfWork unitOfWork, IAuctionService auctionService)
        {
            _unitOfWork = unitOfWork;
            // _breederService = breederService;
            _auctionService = auctionService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllAuction([FromQuery] AuctionQueryObject query)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var auctions = await _unitOfWork.Auctions.GetAllAsync(query);
            var acutionDtos = auctions.Select(d => d.ToAuctionDtoFromAuction());
            return Ok(acutionDtos);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetAuctionById([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var auction = await _unitOfWork.Auctions.GetByIdAsync(id);
            if (auction == null)
                return NotFound();
            return Ok(auction.ToAuctionDtoFromAuction());
        }

        [HttpPost]
        public async Task<IActionResult> CreateAuction([FromBody] CreateAuctionDto auctionDto)
        {

            var uidHeader = HttpContext.Request.Headers["uid"].FirstOrDefault();
            if (string.IsNullOrEmpty(uidHeader) || !int.TryParse(uidHeader, out var uid))
            {
                return BadRequest("Invalid or missing uid header");
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Auction auction = auctionDto.ToAuctionFromCreateAuctionDto(uid);
            auction.AuctionName = AuctionHelper.GenerateAuctionName(auction);
            auction.StartTime = auctionDto.StartTime;

            await _unitOfWork.Auctions.CreateAsync(auction);
            if (!await _unitOfWork.SaveChangesAsync())
            {
                return BadRequest("An error occurred while saving the data");
            }

            // Schedule auction
            _auctionService.ScheduleAuction(auction.AuctionId, auction.StartTime);
            return CreatedAtAction(nameof(GetAuctionById), new { id = auction.AuctionId }, auction.ToAuctionDtoFromAuction());
        }



        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdateAuction([FromRoute] int id, [FromBody] UpdateAuctionDto auctionDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var action = await _unitOfWork.Auctions.UpdateAsync(id, auctionDto);
            await _unitOfWork.SaveChangesAsync();
            return Ok(action.ToAuctionDtoFromAuction());
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<ActionResult> DeleteAuction([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var deletedAuction = await _unitOfWork.Auctions.GetByIdAsync(id);
            if (deletedAuction == null)
                return NotFound();
            var auctionLots = deletedAuction.AuctionLots;
            if (auctionLots != null)
            {
                foreach (var auctionLot in auctionLots)
                {
                    await _unitOfWork.AuctionLots.DeleteAsync(auctionLot.AuctionLotId);
                    await _unitOfWork.Lots.UpdateLotStatusAsync(auctionLot.AuctionLotId,
                                            new Dto.Lot.UpdateLotStatusDto { LotStatusName = "Approved" });
                }
            }
            await _unitOfWork.Auctions.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
            return NoContent();
        }
    }
}