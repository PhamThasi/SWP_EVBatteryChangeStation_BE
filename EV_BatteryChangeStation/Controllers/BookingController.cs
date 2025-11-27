using EV_BatteryChangeStation_Common.DTOs.BookingDTO;
using EV_BatteryChangeStation_Service.InternalService.IService;
// Hiển<Task>: Thêm using cho Authorization và JWT Claims
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        /// <summary>
        /// Lấy danh sách tất cả lịch đổi pin
        /// </summary>
        [HttpGet("SelectAll/")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _bookingService.GetAllAsync();
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Lấy thông tin lịch đổi pin theo ID
        /// </summary>
        [HttpGet("Select/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _bookingService.GetByIdAsync(id);
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Tạo mới lịch đổi pin
        /// </summary>
        [HttpPost("Create/")]
        public async Task<IActionResult> Create([FromBody] BookingCreateDTO dto)
        {
            var result = await _bookingService.CreateAsync(dto);
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Cập nhật lịch đổi pin
        /// </summary>
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] BookingCreateDTO dto)
        {
            var result = await _bookingService.UpdateAsync(id, dto);
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Staff xác nhận hoặc từ chối booking (chuyển trạng thái Pending → Approved/Rejected)
        /// </summary>
        [HttpPut("UpdateStatus")]
        [Authorize]
        public async Task<IActionResult> UpdateBookingStatus([FromBody] UpdateBookingStatusDTO dto)
        {
            if (dto == null || dto.BookingId == Guid.Empty)
                return BadRequest("BookingId and Status are required");

            // Lấy StaffId từ JWT Token
            var staffIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                            ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrEmpty(staffIdClaim) || !Guid.TryParse(staffIdClaim, out Guid staffId))
            {
                return Unauthorized(new { message = "Invalid token", status = 401 });
            }

            var result = await _bookingService.UpdateBookingStatusAsync(dto.BookingId, dto.Status, staffId, dto.Notes);
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Hủy (Soft Delete) — chuyển trạng thái IsApproved thành Canceled
        /// </summary>
        [HttpDelete("Cancel/{id}")]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var result = await _bookingService.DeleteAsync(id);
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Xóa cứng lịch đổi pin
        /// </summary>
        [HttpDelete("HardDelete/{id}")]
        public async Task<IActionResult> HardDelete(Guid id)
        {
            var result = await _bookingService.HardDeleteAsync(id);
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Lấy danh sách lịch đổi pin theo người dùng
        /// </summary>
        [HttpGet("User/{accountId}")]
        public async Task<IActionResult> GetByAccountId(Guid accountId)
        {
            var result = await _bookingService.GetByAccountIdAsync(accountId);
            return StatusCode(result.Status, result);
        }

        // Hiển<Task>: Endpoint để Staff xem booking của Station mà họ đang làm việc
        // Lấy AccountId từ JWT Token, validate và chỉ trả về booking của Station đó
        /// <summary>
        /// Lấy danh sách booking của Station mà Staff đang làm việc
        /// Staff chỉ được xem booking của Station được gán cho mình
        /// </summary>
        [HttpGet("staff/my-bookings")]
        [Authorize] // Yêu cầu đăng nhập
        public async Task<IActionResult> GetMyStationBookings()
        {
            // Hiển<Task>: Lấy AccountId từ JWT Token (Claims)
            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                              ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrEmpty(accountIdClaim) || !Guid.TryParse(accountIdClaim, out Guid accountId))
            {
                return Unauthorized(new { message = "Invalid token", status = 401 });
            }

            var result = await _bookingService.GetByStaffStationAsync(accountId);
            return StatusCode(result.Status, result);
        }
    }
}
