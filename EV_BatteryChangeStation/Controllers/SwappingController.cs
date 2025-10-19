using EV_BatteryChangeStation_Common.DTOs.SwappingtransactionDto;
using EV_BatteryChangeStation_Service.InternalService.IService;
using Microsoft.AspNetCore.Mvc;

namespace EV_BatteryChangeStation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SwappingController : ControllerBase
    {
        private readonly ISwappingService _swappingService;
        public SwappingController(ISwappingService swappingService)
        {
            _swappingService = swappingService;
        }

        /// <summary>
        /// create swapping
        /// </summary>

        [HttpPost("CreateSwapping")]
        public async Task<IActionResult> Create([FromBody] CreateSwappingDto createSwap)
        {
            if (createSwap == null)
                return BadRequest("Invalid swapping data");
            var result = await _swappingService.CreateTransactionAsync(createSwap);
            if (result.Status == 200)
                return Ok(result);
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// update swapping
        /// </summary>

        [HttpPut("UpdateSwapping")]
        public async Task<IActionResult> Update([FromBody] UpdateSwappingDto updateSwap)
        {
            if (updateSwap == null)
                return BadRequest("Invalid swapping data");
            var result = await _swappingService.UpdateTransactionAsync(updateSwap);
            if (result.Status == 200)
                return Ok(result);
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// get all swapping
        /// </summary>

        [HttpGet("GetAllSwapping")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _swappingService.GetAllTransactionsAsync();
            if (result.Status == 200)
                return Ok(result);
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// get swapping by car id
        /// </summary>

        [HttpGet("GetSwappingByCarId")]
        public async Task<IActionResult> GetByCarId([FromQuery] string carid)
        {
            var result = await _swappingService.GetTransactionByCarIdAsync(carid);
            if (result.Status == 200)
                return Ok(result);
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// get swapping by transaction id
        /// </summary>

        [HttpGet("GetSwappingById")]
        public async Task<IActionResult> GetById([FromQuery] string transactionId)
        {
            var result = await _swappingService.GetTransactionByIdAsync(transactionId);
            if (result.Status == 200)
                return Ok(result);
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// delete swapping
        /// </summary>
        [HttpDelete("DeleteSwapping")]
        public async Task<IActionResult> Delete([FromQuery] string transactionId)
        {
            var result = await _swappingService.DeleteTransactionAsync(transactionId);
            if (result.Status == 200)
                return Ok(result);
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// soft delete swapping
        /// </summary>
        [HttpDelete("SoftDeleteSwapping")]
        public async Task<IActionResult> SoftDelete([FromQuery] string transactionId)
        {
            var result = await _swappingService.SoftDeleteTransactionAsync(transactionId);
            if (result.Status == 200)
                return Ok(result);
            return StatusCode(result.Status, result);
        }


    }
}
