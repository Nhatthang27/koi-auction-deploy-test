using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionService.Helper;
using AuctionService.IServices;
using AuctionService.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuctionService.Controller
{
    [ApiController]
    [Route("api/sold-lots")]
    public class SoldLotController : ControllerBase
    {
        //get sold lot by id
        private readonly ISoldLotService _service;
        public SoldLotController(ISoldLotService service, BreederDetailService breederService, UserSevice userSevice)
        {
            _service = service;

        }

        [HttpGet]
        public async Task<IActionResult> GetAllSoldLot([FromQuery] SoldLotQueryObject queryObject)
        {
            var soldLots = await _service.GetAllAsync(queryObject);
            return Ok(soldLots);
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetSoldLotById([FromRoute] int id)
        {
            var soldLot = await _service.GetSoldLotById(id);
            return Ok(soldLot);
        }
    }
}