using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using EV_BatteryChangeStation_Common.DTOs.BatteryDTO;
using EV_BatteryChangeStation_Service.InternalService.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EV_BatteryChangeStation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BatteryController : ControllerBase
    {
        private readonly IBatteryService _batteryService;
        public BatteryController(IBatteryService batteryService)
        {
            _batteryService = batteryService ?? throw new ArgumentNullException(nameof(batteryService));
        }

        ///<summary>
        ///Create new Battery
        ///</summary>
        ///<param name="dto">Thông tin pin cần tạo</param>

        [HttpPost("CreateBattery")]
        public async Task<IActionResult> CreateBattery([FromBody] CreateBatteryDTO dto)
        {
            if (dto == null)
                return BadRequest("Invalid battery data");
            var result = await _batteryService.CreateBatteryAsync(dto);
            if (result.Status == 200)
                return Ok(result);
            return StatusCode(result.Status, result.Message);
        }

        /// <summary>
        /// Update Battery
        /// </summary>
        /// <param name="dto">Thông tin pin cần cập nhật</param>

        [HttpPut("UpdateBattery")]
        public async Task<IActionResult> UpdateBattery([FromBody] UpdateBattery dto)
        {
            if (dto == null)
                return BadRequest("Invalid battery data");
            var result = await _batteryService.UpdateBatteryAsync(dto);
            if (result.Status == 200)
                return Ok(result);
            return StatusCode(result.Status, result.Message);
        }

        /// <summary>
        /// Lấy tất cả pin
        /// </summary>
        /// <param name="dto">Lấy thông tin tất cả pin</param>
        [HttpGet("GetAllBattery")]
        public async Task<IActionResult> GetAllBattery()
        {
            var result = await _batteryService.GetAllBattery();
            if (result.Status == 200)
                return Ok(result);
            return StatusCode(result.Status, result.Message);
        }

        /// <summary>
        /// Lấy pin bằng id
        /// </summary>
        /// <param name="dto">Lấy thông tin pin theo id</param>
        [HttpGet("GetBatteryById")]
        public async Task<IActionResult> GetBatteryById([FromQuery] Guid batteryId)
        {
            if (batteryId == Guid.Empty)
                return BadRequest("Invalid battery data");
            var result = await _batteryService.GetBatteryById(batteryId);
            if (result.Status == 200)
                return Ok(result);
            return StatusCode(result.Status, result.Message);
        }

        /// <summary>
        /// Lấy tất cả pin theo station id
        /// </summary>
        [HttpGet("GetBatteryByStationId")]
        public async Task<IActionResult> GetBatteryByStationId([FromQuery] Guid stationId)
        {
            if (stationId == Guid.Empty)
                return BadRequest("Invalid station data");
            var result = await _batteryService.GetAllBatteryByStationId(stationId);
            if (result.Status == 200)
                return Ok(result);
            return StatusCode(result.Status, result.Message);
        }

        /// <summary>
        /// Lấy số lượng pin theo station id
        /// </summary>
        [HttpGet("GetBatteryCountByStationId")]
        public async Task<IActionResult> GetBatteryCountByStationId([FromQuery] Guid stationId)
        {
            if (stationId == Guid.Empty)
                return BadRequest("Invalid station data");
            var result = await _batteryService.GetBatteryCountByStationId(stationId);
            if (result.Status == 200)
                return Ok(result);
            return StatusCode(result.Status, result.Message);
        }

        /// <summary>
        /// Kiểm tra pin có khả năng thay không
        /// </summary>
        [HttpGet("CheckBattery")]
        public async Task<IActionResult> CheckBattery([FromQuery] Guid batteryId)
        {
            if (batteryId == Guid.Empty)
                return BadRequest("Invalid battery data");
            var result = await _batteryService.IsBatteryAvailable(batteryId);
            if (result.Status == 200)
                return Ok(result);
            return StatusCode(result.Status, result.Message);
        }

        /// <summary>
        /// Xóa pin vĩnh viễn
        /// </summary>
        [HttpDelete("DeleteBattery")]
        public async Task<IActionResult> DeleteBattery([FromQuery] Guid batteryId)
        {
            if (batteryId == Guid.Empty)
                return BadRequest("Invalid battery data");
            var result = await _batteryService.DeleteBattery(batteryId);
            if (result.Status == 200)
                return Ok(result);
            return StatusCode(result.Status, result.Message);
        }

        /// <summary>
        /// Xóa pin tạm thời bằng cách đổi trạng thái
        /// </summary>
        [HttpDelete("SoftDelete")]
        public async Task<IActionResult> SoftDeleteBattery([FromQuery] Guid batteryId)
        {
            if (batteryId == Guid.Empty)
                return BadRequest("Invalid battery data");
            var result = await _batteryService.SoftDeleteBattery(batteryId);
            if (result.Status == 200)
                return Ok(result);
            return StatusCode(result.Status, result.Message);
        }

        /// <summary>
        /// get batteries by type
        /// </summary>
        [HttpGet("GetBatteriesByType")]
        public async Task<IActionResult> GetBatteriesByType([FromQuery] string typeBattery)
        {
            if (string.IsNullOrEmpty(typeBattery))
                return BadRequest("Invalid battery type data");
            var result = await _batteryService.GetBatteriesByType(typeBattery);
            if (result.Status == 200)
                return Ok(result);
            return StatusCode(result.Status, result.Message);

        }

        /// <summary>
        /// Preview pin sẽ được gán khi tạo booking
        /// Frontend gọi API này để hiển thị cho user biết pin nào sẽ được gán trước khi tạo booking
        /// </summary>
        /// <param name="stationId">ID của trạm được chọn</param>
        /// <param name="vehicleId">ID của xe được chọn</param>
        [HttpGet("PreviewForBooking")]
        public async Task<IActionResult> PreviewBatteryForBooking([FromQuery] Guid stationId, [FromQuery] Guid vehicleId)
        {
            if (stationId == Guid.Empty)
                return BadRequest("StationId is required");
            if (vehicleId == Guid.Empty)
                return BadRequest("VehicleId is required");

            var result = await _batteryService.PreviewBatteryForBookingAsync(stationId, vehicleId);
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Lấy danh sách pin của Station mà Staff đang làm việc
        /// Staff chỉ được xem pin của Station được gán cho mình
        /// </summary>
        [HttpGet("staff/my-station-batteries")]
        [Authorize]
        public async Task<IActionResult> GetMyStationBatteries()
        {
            // Lấy AccountId từ JWT Token (Claims)
            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrEmpty(accountIdClaim) || !Guid.TryParse(accountIdClaim, out Guid accountId))
            {
                return Unauthorized(new { message = "Invalid token", status = 401 });
            }

            var result = await _batteryService.GetBatteriesByStaffStationAsync(accountId);
            return StatusCode(result.Status, result);
        }
    }
}
