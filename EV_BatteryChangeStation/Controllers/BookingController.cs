using EV_BatteryChangeStation_Common.DTOs.BookingDTO;
using EV_BatteryChangeStation_Service.InternalService.IService;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> GetById(int id)
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
        public async Task<IActionResult> Update(int id, [FromBody] BookingCreateDTO dto)
        {
            var result = await _bookingService.UpdateAsync(id, dto);
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Xóa lịch đổi pin
        /// </summary>
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _bookingService.DeleteAsync(id);
            return StatusCode(result.Status, result);
        }
        [HttpDelete("HardDelete/{id}")]
        public async Task<IActionResult> HardDelete(int id)
        {
            var result = await _bookingService.HardDeleteAsync(id);
            return StatusCode(result.Status, result);
        }

    }
}
