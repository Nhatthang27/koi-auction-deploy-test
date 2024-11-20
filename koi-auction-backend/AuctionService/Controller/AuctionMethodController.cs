

using AuctionService.Dto;
using AuctionService.IRepository;
using AuctionService.Mapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionService.Controller
{
    [Route("api/auction-methods")]
    [ApiController]
    public class AuctionMethodController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public AuctionMethodController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var methods = await _unitOfWork.AuctionMethods.GetAllAsync();
            var methodDtos = methods.Select(m => m.ToAuctionMethodDtoFromAuctionMethod());
            return Ok(methodDtos);
        }
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetAuctionMethodByID([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var method = await _unitOfWork.AuctionMethods.GetByIdAsync(id);
            if (method == null)
            {
                return NotFound();
            }
            return Ok(method.ToAuctionMethodDtoFromAuctionMethod());
        }

        [HttpPost]
        public async Task<IActionResult> CreateMethodAuction([FromBody] CreateAuctionMethodDto methodDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var method = methodDto.ToActionMethodFromCreateAuctionMethodDto();
            var newMethod = await _unitOfWork.AuctionMethods.CreateAsync(method);
            await _unitOfWork.SaveChangesAsync(); ;
            return CreatedAtAction(nameof(GetAuctionMethodByID), new { id = newMethod.AuctionMethodId }, newMethod.ToAuctionMethodDtoFromAuctionMethod());
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdateMethodAuction([FromRoute] int id, [FromBody] UpdateAuctionMethodDto methodDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var updateMethod = await _unitOfWork.AuctionMethods.UpdateAsync(id, methodDto);
            await _unitOfWork.SaveChangesAsync();
            return Ok(updateMethod.ToAuctionMethodDtoFromAuctionMethod());
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteMethodAuction([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var deleteMethod = await _unitOfWork.AuctionMethods.DeleteAsync(id);
            if (deleteMethod == null)
                return NotFound();
            await _unitOfWork.SaveChangesAsync();
            return NoContent();
        }


    }


}