using AuctionService.IRepository;
using AuctionService.Mapper;
using Microsoft.AspNetCore.Mvc;

namespace AuctionService.Controller
{
    [Route("api/auction-statuses")]
    [ApiController]

    public class AuctionStatusesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public AuctionStatusesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAuctionStatuses()
        {
            var AuctionStatuses = await _unitOfWork.AuctionStatuses.GetAllAsync();
            var AuctionStatusesDto = AuctionStatuses.Select(s => s.ToAuctionStatusDtoFromAuctionStatus());
            return Ok(AuctionStatusesDto);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetAuctionStatusesById([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var AuctionStatuses = await _unitOfWork.AuctionStatuses.GetAuctionStatusByIdAsync(id);
            return Ok(AuctionStatuses.ToAuctionStatusDtoFromAuctionStatus());
        }
    }
}