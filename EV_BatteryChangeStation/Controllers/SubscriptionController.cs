using EV_BatteryChangeStation_Common.DTOs.SubscriptionDTO;
using EV_BatteryChangeStation_Service.InternalService.IService;
using Microsoft.AspNetCore.Mvc;

namespace EV_BatteryChangeStation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [HttpGet("SelectAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _subscriptionService.GetAllAsync();
            return StatusCode(result.Status, result);
        }

        [HttpGet("Select/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _subscriptionService.GetByIdAsync(id);
            return StatusCode(result.Status, result);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] SubscriptionCreateUpdateDTO dto)
        {
            var result = await _subscriptionService.CreateAsync(dto);
            return StatusCode(result.Status, result);
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] SubscriptionCreateUpdateDTO dto)
        {
            var result = await _subscriptionService.UpdateAsync(id, dto);
            return StatusCode(result.Status, result);
        }

        [HttpDelete("SoftDelete/{id}")]
        public async Task<IActionResult> SoftDelete(Guid id)
        {
            var result = await _subscriptionService.SoftDeleteAsync(id);
            return StatusCode(result.Status, result);
        }

        [HttpDelete("HardDelete/{id}")]
        public async Task<IActionResult> HardDelete(Guid id)
        {
            var result = await _subscriptionService.HardDeleteAsync(id);
            return StatusCode(result.Status, result);
        }
        [HttpPut("Restore/{id}")]
        public async Task<IActionResult> Restore(Guid id)
        {
            var result = await _subscriptionService.RestoreAsync(id);
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Lấy subscription đang active của user (nếu có)
        /// </summary>
        [HttpGet("my-active")]
        public async Task<IActionResult> GetMyActiveSubscription([FromQuery] Guid accountId)
        {
            if (accountId == Guid.Empty)
                return BadRequest("AccountId is required");

            var result = await _subscriptionService.GetActiveByAccountIdAsync(accountId);
            return StatusCode(result.Status, result);
        }
    }
}
