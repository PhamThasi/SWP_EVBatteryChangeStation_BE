using EV_BatteryChangeStation_Common.DTOs.FeedBackDTO;
using EV_BatteryChangeStation_Service.InternalService.IService;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedBackController : ControllerBase
    {
        private readonly IFeedBackService _feedBackService;

        public FeedBackController(IFeedBackService feedBackService)
        {
            _feedBackService = feedBackService;
        }

        // Lấy tất cả feedback
        [HttpGet("SelectAll")]
        public async Task<IActionResult> GetAll()
        {
            var data = await _feedBackService.GetAllAsync();
            return Ok(new
            {
                status = 200,
                message = "Get all feedbacks successfully",
                data
            });
        }

        // Lấy feedback theo ID
        [HttpGet("Select/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var feedback = await _feedBackService.GetByIdAsync(id);
            if (feedback == null)
                return NotFound(new
                {
                    status = 404,
                    message = "Feedback not found"
                });

            return Ok(new
            {
                status = 200,
                message = "Get feedback successfully",
                data = feedback
            });
        }

        // Tạo feedback mới (hiển thị JSON có status + message + data)
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreateFeedBackDTO dto)
        {
            var created = await _feedBackService.CreateAsync(dto);

            return StatusCode(201, new
            {
                status = 201,
                message = "Feedback created successfully",
                data = created
            });
        }

        // Cập nhật feedback
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateFeedBackDTO dto)
        {
            var updated = await _feedBackService.UpdateAsync(id, dto);

            return Ok(new
            {
                status = 200,
                message = "Feedback updated successfully",
                data = updated
            });
        }

        // Xóa feedback (xóa cứng)
        [HttpDelete("HardDelete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _feedBackService.DeleteAsync(id);
            return Ok(new
            {
                status = 200,
                message = "Feedback deleted successfully"
            });
        }
        // Lấy feedback theo AccountId
        [HttpGet("SelectByAccount/{accountId}")]
        public async Task<IActionResult> GetByAccountId(Guid accountId)
        {
            var result = await _feedBackService.GetByAccountIdAsync(accountId);

            if (result.Status == 404)
                return NotFound(new
                {
                    status = 404,
                    message = $"No feedbacks found for Account ID = {accountId}"
                });

            if (result.Status != 200)
                return StatusCode(result.Status, new
                {
                    status = result.Status,
                    message = result.Message
                });

            return Ok(new
            {
                status = 200,
                message = "Get feedbacks by account successfully",
                data = result.Data
            });
        }

    }
}
