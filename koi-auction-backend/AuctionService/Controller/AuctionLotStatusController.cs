
using AuctionService.IRepository;
using AuctionService.Mapper;
using Microsoft.AspNetCore.Mvc;

namespace AuctionService.Controller
{
    [Route("api/auction-lot-statuses")]
    [ApiController]
    public class AuctionLotStatusesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public AuctionLotStatusesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAuctionLotStatuses()
        {
            var AuctionLotStatuses = await _unitOfWork.AuctionLotStatuses.GetAllAsync();
            var AuctionLotStatusesDto = AuctionLotStatuses.Select(s => s.ToAuctionLotStatusDtoFromAuctionLotStatus());
            return Ok(AuctionLotStatusesDto);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetAuctionLotStatusesById([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var AuctionLotStatuses = await _unitOfWork.AuctionLotStatuses.GetAuctionLotStatusByIdAsync(id);
            return Ok(AuctionLotStatuses.ToAuctionLotStatusDtoFromAuctionLotStatus());

        }
    }
}