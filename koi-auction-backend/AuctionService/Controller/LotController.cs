using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AuctionService.Dto.AuctionMethod;
using AuctionService.Dto.BreederDetail;
using AuctionService.Dto.Dashboard;
using AuctionService.Dto.KoiFish;
using AuctionService.Dto.Lot;
using AuctionService.Dto.LotRequestForm;
using AuctionService.Dto.LotStatus;
using AuctionService.Helper;
using AuctionService.IRepository;
using AuctionService.IServices;
using AuctionService.Mapper;
using AuctionService.Models;
using AuctionService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace AuctionService.Controller
{
    [Route("api/lots")]
    [ApiController]
    public class LotController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        // private readonly BreederDetailController _breederDetailController;

        private readonly BreederDetailService _breederService;
        private readonly ILotService _lotService;
        public LotController(IUnitOfWork unitOfWork, BreederDetailService service, ILotService lotService)
        {
            _unitOfWork = unitOfWork;
            _breederService = service;
            _lotService = lotService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLot([FromQuery] LotQueryObject query)
        {

            var lots = await _unitOfWork.Lots.GetAllAsync(query);
            var breeders = await _breederService.GetAllBreederAsync();
            Dictionary<int, BreederDetailDto> breederCache = new();
            breeders.ForEach(b => breederCache[b.BreederId] = b);
            // Tạo LotDto và gán thông tin người dùng
            var tasks = lots.Select(lot =>
            {
                // var breeder = breeders.FirstOrDefault(b => b.BreederId == lot.BreederId);
                var breeder = breederCache[lot.BreederId];
                var lotDto = lot.ToLotDtoFromLot();
                lotDto.BreederDetailDto = breeder;
                return lotDto;
            }).ToList();

            // Đợi tất cả các tác vụ hoàn thành
            return Ok(tasks);
        }

        [HttpGet]
        [ActionName(nameof(GetLotById))]
        [Route("{id:int}")]
        public async Task<IActionResult> GetLotById([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var lot = await _unitOfWork.Lots.GetLotByIdAsync(id);

            var lotDto = lot.ToLotDtoFromLot();
            lotDto.BreederDetailDto = await _breederService.GetBreederByIdAsync(lot.BreederId);

            return Ok(lotDto);
        }

        [HttpGet("search-koi")]
        public async Task<IActionResult> GetLotSearchResult([FromQuery] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _lotService.GetLotSearchResults(id);
            return Ok(result);
        }

        [HttpGet("revenue-statistics")]
        public async Task<ActionResult<List<DailyRevenueDto>>> GetStatisticsRevenue([FromQuery] DateTime startDateTime, [FromQuery] DateTime endDateTime)
        {
            var revenueData = await _lotService.GetStatisticsRevenue(startDateTime, endDateTime);

            if (revenueData == null || revenueData.Count == 0)
            {
                return NotFound("No revenue data found for the specified period.");
            }

            return Ok(revenueData);
        }

        [HttpPost]
        public async Task<IActionResult> CreateLot([FromBody] CreateLotRequestFormDto lotRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //add lot
            var newLot = lotRequest.ToLotFromCreateLotRequestFormDto();
            newLot.Sku = LotHelper.GenerateSku(newLot);
            var modelLot = await _unitOfWork.Lots.CreateLotAsync(newLot);

            //add koifish
            var newKoiFish = lotRequest.ToKoiFishFromCreateLotRequestFormDto();
            modelLot.KoiFish = newKoiFish;

            //add media
            var newKoiMedia = lotRequest.KoiMedia.Select(m => m.ToKoiMediaFromFormKoiMediaDto());
            if (newKoiMedia == null)
                return NotFound();
            foreach (var media in newKoiMedia)
            {
                newKoiFish.KoiMedia.Add(media);
            }

            await _unitOfWork.SaveChangesAsync();
            return CreatedAtAction(nameof(GetLotById), new { id = newLot.LotId }, newLot.ToLotDtoFromLot());
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdateLot([FromRoute] int id, [FromBody] UpdateLotRequestFormDto lotRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //update lot
            var updateLotDto = lotRequest.ToUpdateLotDtoFromLotRequestFormDto();
            var updateLot = await _unitOfWork.Lots.UpdateLotAsync(id, updateLotDto);

            //update koifish
            var updateKoiFishDto = lotRequest.ToUpdateKoiFishDtoFromUpdateLotRequestFormDto();
            var updateKoiFish = await _unitOfWork.KoiFishes.UpdateKoiAsync(id, updateKoiFishDto);

            //update media
            await _unitOfWork.KoiMedia.DeleteKoiMediaAsync(id);
            var updateKoiMedia = lotRequest.KoiMedia;
            if (updateKoiMedia == null)
            {
                return BadRequest();
            }
            foreach (var media in updateKoiMedia)
            {
                var newMedia = media.ToKoiMediaFromFormKoiMediaDto();
                newMedia.KoiFishId = id;
                await _unitOfWork.KoiMedia.CreateKoiMediaAsync(newMedia);
            }
            await _unitOfWork.SaveChangesAsync();
            return Ok(updateLot.ToLotDtoFromLot());
        }

        [HttpPut("{id:int}/status")]
        public async Task<IActionResult> UpdateLotStatus(int id, UpdateLotStatusDto lotStatusDto)
        {
            var updateLot = await _unitOfWork.Lots.UpdateLotStatusAsync(id, lotStatusDto);
            if (updateLot == null)
                return BadRequest();
            await _unitOfWork.SaveChangesAsync();
            return Ok(updateLot.ToLotDtoFromLot());
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteLot([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var deleteLot = await _unitOfWork.Lots.DeleteLotAsync(id);
            if (deleteLot == null)
                return NotFound();
            await _unitOfWork.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("auction-method-statistics")]
        public async Task<ActionResult<List<LotAuctionMethodStatisticDto>>> GetAuctionMethodStatistics()
        {
            try
            {
                var statistics = await _lotService.GetLotAuctionMethodStatisticAsync();
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error retrieving auction method statistics", error = ex.Message });
            }
        }

        [HttpGet("breeder-statistics")]
        public async Task<ActionResult<List<BreederStatisticDto>>> GetBreederStatistics()
        {
            try
            {
                var statistics = await _lotService.GetBreederStatisticsAsync();
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error retrieving breeder statistics", error = ex.Message });
            }
        }

        [HttpGet("total-statistics")]
        public async Task<ActionResult<TotalDto>> GetTotalLotsStatisticsAsync([FromQuery] int? breederId, [FromQuery] DateTime startDateTime, [FromQuery] DateTime endDateTime)
        {
            var result = await _lotService.GetTotalLotsStatisticsAsync(breederId, startDateTime, endDateTime);
            return Ok(result);
        }
    }
}