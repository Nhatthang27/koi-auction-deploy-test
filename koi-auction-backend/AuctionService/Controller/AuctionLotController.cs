using AuctionService.Dto.AuctionLot;
using AuctionService.Dto.BreederDetail;
using AuctionService.Helper;
using AuctionService.IRepository;
using AuctionService.IServices;
using AuctionService.Mapper;
using AuctionService.Models;
using AuctionService.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace AuctionService.Controller
{
    [Route("api/auction-lots")]
    [ApiController]
    public class AuctionLotController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly BreederDetailService _breederService;
        private readonly IAuctionLotService _auctionLotService;
        public AuctionLotController(IUnitOfWork unitOfWork, BreederDetailService breederService, IAuctionLotService auctionLotService)
        {
            _unitOfWork = unitOfWork;
            _auctionLotService = auctionLotService;
            _breederService = breederService;
        }

        [HttpPost]
        [Route("schedule-auction-lot")]
        public async Task<ActionResult> StartAuctionLot([FromBody] ScheduleAuctionLotDto scheduleAuctionLotDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                System.Console.WriteLine($"Receive ");
                await _auctionLotService.ScheduleAuctionLotAsync(scheduleAuctionLotDto.AuctionLotId, scheduleAuctionLotDto.StartTime);
                return Ok(new { message = "Auction lot is scheduled!" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAuctionLot([FromQuery] AuctionLotQueryObject query)
        {
            var auctionLots = await _unitOfWork.AuctionLots.GetAllAsync(query);
            var breeders = await _breederService.GetAllBreederAsync();
            Dictionary<int, BreederDetailDto> breederCache = new();
            breeders.ForEach(b => breederCache[b.BreederId] = b);
            var tasks = auctionLots.Select(auctionLot =>
            {
                var auctionLotDto = auctionLot.ToAuctionLotDtoFromAuctionLot();
                // auctionLotDto!.LotDto!.BreederDetailDto = await _breederService.GetBreederByIdAsync(auctionLotDto.LotDto.BreederId);
                auctionLotDto!.LotDto!.BreederDetailDto = breederCache[auctionLotDto.LotDto.BreederId];
                return auctionLotDto;
            }).ToList();

            return Ok(tasks);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetAuctionLotById([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var auctionLot = await _unitOfWork.AuctionLots.GetAuctionLotById(id);
            if (auctionLot == null)
                return NotFound();
            var auctionLotDto = auctionLot.ToAuctionLotDtoFromAuctionLot();
            auctionLotDto!.LotDto!.BreederDetailDto = await _breederService.GetBreederByIdAsync(auctionLotDto.LotDto.BreederId);
            return Ok(auctionLotDto);
        }

        [HttpPost]
        public async Task<ActionResult> CreateAuctionLot([FromBody] CreateAuctionLotDto auctionLotDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var newAuctionLot = await _auctionLotService.CreateAsync(auctionLotDto);
            return CreatedAtAction(nameof(GetAuctionLotById), new { id = newAuctionLot.AuctionLotId }, newAuctionLot);
        }


        // call from auction service to end auction lot
        // [HttpPut]
        // [Route("endAuctionLot/{auctionLotId:int}")]
        // public async Task<ActionResult> EndAuctionLot([FromRoute] int auctionLotId, [FromBody] EndAuctionLotDto endAuctionLotDto)
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         return BadRequest(ModelState);
        //     }
        //     var auctionLot = await _unitOfWork.AuctionLots.GetAuctionLotById(auctionLotId);
        //     if (auctionLot == null)
        //     {
        //         return NotFound();
        //     }
        //     await _auctionLotService.EndAuctionLot(auctionLot.AuctionLotId, endAuctionLotDto.EndTime);
        //     return Ok(auctionLot.ToAuctionLotDtoFromAuctionLot());
        // }
        // [HttpPut]
        // [Route("updateEndTime/{auctionLotId:int}")]
        // public async Task<ActionResult> UpdateEndTimeAuctionLot([FromRoute] int auctionLotId, [FromBody] UpdateEndTimeAuctionLotDto updateEndTimeAuctionLotDto)
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         return BadRequest(ModelState);
        //     }
        //     await _auctionLotService.UpdateEndTimeAuctionLot(auctionLotId, updateEndTimeAuctionLotDto.EndTime);
        //     return Ok();
        // }

        // [HttpPost]
        // [Route("test-start-auction-lot")]
        // public async Task<ActionResult> TestStartAuctionLot([FromBody] TestStartAuctionLotDto testStartAuctionLotDto)
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         return BadRequest(ModelState);
        //     }
        //     try
        //     {
        //         await _auctionLotService.StartAuctionLot(testStartAuctionLotDto.AuctionLotId);
        //         return Ok(new { message = "Auction lot is starting!" });
        //     }
        //     catch (Exception ex)
        //     {
        //         return BadRequest(ex.Message);
        //     }
        // }

        [HttpPost("listAuctionLot")]
        public async Task<ActionResult> CreateListAuctionLot([FromBody] List<CreateAuctionLotDto> listAuctionLotDto)
        {
            if (!ModelState.IsValid || listAuctionLotDto.Count == 0)
            {
                return BadRequest(ModelState);
            }

            //print all list
            // foreach (var auctionLot in listAuctionLotDto)
            // {
            //     System.Console.WriteLine($"Auction Id: {auctionLot.AuctionId}");
            //     System.Console.WriteLine($"Auction Lot: {auctionLot.AuctionLotId}");
            //     System.Console.WriteLine($"Order in Auction: {auctionLot.OrderInAuction}");
            //     System.Console.WriteLine($"Duration: {auctionLot.Duration}");
            //     System.Console.WriteLine($"Step Percent: {auctionLot.StepPercent}");
            // }

            //create auction lot list    
            await _auctionLotService.CreateListAsync(listAuctionLotDto);

            //schedule auction lot list
            int auctionId = listAuctionLotDto.First().AuctionId;
            Auction auction = await _unitOfWork.Auctions.GetByIdAsync(auctionId);
            DateTime startTime = auction.StartTime;
            foreach (var auctionLot in listAuctionLotDto)
            {
                await _auctionLotService.ScheduleAuctionLotAsync(auctionLot.AuctionLotId, startTime);
            }
            return StatusCode(201);
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<ActionResult> UpdateAuctionLot([FromRoute] int id, [FromBody] UpdateAuctionLotDto auctionLotDto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var auctionLot = await _unitOfWork.AuctionLots.GetAuctionLotById(id);
            _unitOfWork.AuctionLots.Update(auctionLot, auctionLotDto);
            await _unitOfWork.SaveChangesAsync();
            return Ok(auctionLot.ToAuctionLotDtoFromAuctionLot());
        }

        [HttpPatch]
        [Route("{id:int}")]
        public async Task<ActionResult> PatchAuctionLot([FromRoute] int id, [FromBody] JsonPatchDocument<UpdateAuctionLotDto> patchDoc)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var auctionLot = await _unitOfWork.AuctionLots.GetAuctionLotById(id);
            var updateAuctionLotDto = auctionLot.ToUpdateAuctionLotDtoFromAuctionLot();
            patchDoc.ApplyTo(updateAuctionLotDto, ModelState);
            if (!TryValidateModel(updateAuctionLotDto))
            {
                return ValidationProblem(ModelState);
            }
            _unitOfWork.AuctionLots.Update(auctionLot, updateAuctionLotDto);
            await _unitOfWork.SaveChangesAsync();
            return Ok(auctionLot.ToAuctionLotDtoFromAuctionLot());
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<ActionResult> DeleteAuctionLot([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await _auctionLotService.DeleteAsync(id);
            return NoContent();
        }

        [HttpDelete("listAuctionLot")]
        public async Task<ActionResult> DeleteListAuctionLot([FromBody] List<int> ids)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (ids == null)
            {
                return BadRequest("List is empty");
            }

            var isDeleteList = await _auctionLotService.DeleteListAsync(ids);

            if (isDeleteList) return NoContent();
            return NotFound();
        }
    }

}