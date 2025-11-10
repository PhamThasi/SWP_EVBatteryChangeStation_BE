using EV_BatteryChangeStation_Common.DTOs.CarDTO;
using EV_BatteryChangeStation_Service.InternalService.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace EV_BatteryChangeStation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarController : ControllerBase
    {
        private readonly ICarService _carService;
        public CarController(ICarService carService)
        {
            _carService = carService ?? throw new ArgumentNullException(nameof(carService));
        }
        /// <summary>
        /// Create new Car
        /// </summary>

        [HttpPost("CreateCar")]
        public async Task<IActionResult> CreateCar([FromBody] CreateCarDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid car data");
            var result = await _carService.AddCarAsync(dto);
            if (result.Status == 200)
                return Ok(result);
            return StatusCode(result.Status, result.Message);
        }

        /// <summary>
        /// Update Car
        /// </summary>
        [HttpPut("UpdateCar")]
        public async Task<IActionResult> UpdateCar([FromBody] UpdateCarDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid car data");
            var result = await _carService.UpdateCarAsync(dto);
            if (result.Status == 200)
                return Ok(result);
            return StatusCode(result.Status, result.Message);
        }

        /// <summary>
        /// Get All Cars
        /// </summary>
        [HttpGet("GetAllCar")]
        public async Task<IActionResult> GetAllCar()
        {
            var result = await _carService.GetAllCarsAsync();
            if (result.Status == 200)
                return Ok(result);
            return StatusCode(result.Status, result.Message);
        }

        /// <summary>
        /// Get Car By Id
        /// </summary>
        [HttpGet("GetCarById")]
        public async Task<IActionResult> GetCarById([FromQuery] Guid carId)
        {
            if (carId == Guid.Empty)
                return BadRequest("Invalid car id");
            var result = await _carService.GetCarByIdAsync(carId);
            if (result.Status == 200)
                return Ok(result);
            return StatusCode(result.Status, result.Message);
        }

        /// <summary>
        /// Get Account's Car
        /// </summary>
        [HttpGet("GetOwnerByCarIdAsync")]
        public async Task<IActionResult> GetOwnerByCarIdAsync([FromQuery] Guid carId)
        {
            if (carId == Guid.Empty)
                return BadRequest("Invalid Car id");
            var result = await _carService.GetOwnerByCarIdAsync(carId);
            if (result.Status == 200)
                return Ok(result);
            return StatusCode(result.Status, result.Message);
        }

        /// <summary>
        /// Delete Car
        /// </summary>
        [HttpDelete("DeleteCar")]
        public async Task<IActionResult> DeleteCar([FromQuery] Guid carId)
        {
            if (carId == Guid.Empty)
                return BadRequest("Invalid car id");
            var result = await _carService.DeleteCarAsync(carId);
            if (result.Status == 200)
                return Ok(result);
            return StatusCode(result.Status, result.Message);
        }

        /// <summary>
        /// SoftDelete
        /// </summary>
        [HttpDelete("SoftDeleteCar")]
        public async Task<IActionResult> SoftDeleteCar([FromQuery] Guid carId)
        {
            if (carId == Guid.Empty)
                return BadRequest("Invalid car id");
            var result = await _carService.SoftDeleteCarAsync(carId);
            if (result.Status == 200)
                return Ok(result);
            return StatusCode(result.Status, result.Message);
        }

        /// <summary>
        /// get car by model name
        /// </summary>
        [HttpGet("GetCarByName")]
        public async Task<IActionResult> GetCarByName([FromQuery] string modelName)
        {
            if (string.IsNullOrEmpty(modelName))
                return BadRequest("Invalid model name");
            var result = await _carService.GetCarByNameAsync(modelName);
            if (result.Status == 200)
                return Ok(result);
            return StatusCode(result.Status, result.Message);
        }


        /// <summary>
        /// get car by owner id
        /// </summary>

        [HttpGet("GetCarByOwner")]
        public async Task<IActionResult> GetCarsByOwnerId([FromQuery] Guid ownerId)
        {
            if (ownerId == Guid.Empty)
                return BadRequest("Invalid owner id");
            var result = await _carService.GetCarsByOwnerIdAsync(ownerId);
            if (result.Status == 200)
                return Ok(result);
            return StatusCode(result.Status, result.Message);
        }

        /// <summary>
        /// Search batteries by car batterytype
        ///</summary>
        ///
        [HttpGet("GetBatteryByCarBatteryType")]
        public async Task<IActionResult> GetBatteriesByCarAsync([FromQuery] Guid vehicleId)
        {
            if (vehicleId == Guid.Empty)
                return BadRequest("Invalid vehicle id");
            var result = await _carService.GetBatteriesByCarAsync(vehicleId);
            if (result.Status == 200)
                return Ok(result);
            return StatusCode(result.Status, result.Message);
        }
    }
}
