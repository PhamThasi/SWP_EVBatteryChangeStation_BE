using EV_BatteryChangeStation_Common.DTOs.SupportRequestDTO;
using EV_BatteryChangeStation_Service.InternalService.IService;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupportRequestController : ControllerBase
    {
        private readonly ISupportRequestService _service;

        public SupportRequestController(ISupportRequestService service)
        {
            _service = service;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return StatusCode(result.Status, result);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            return StatusCode(result.Status, result);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] SupportRequestCreateDTO dto)
        {
            var result = await _service.CreateAsync(dto);
            return StatusCode(result.Status, result);
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] SupportRequestUpdateDTO dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            return StatusCode(result.Status, result);
        }

        [HttpPut("SoftDelete/{id}")]
        public async Task<IActionResult> SoftDelete(Guid id)
        {
            var result = await _service.SoftDeleteAsync(id);
            return StatusCode(result.Status, result);
        }

        [HttpDelete("HardDelete/{id}")]
        public async Task<IActionResult> HardDelete(Guid id)
        {
            var result = await _service.HardDeleteAsync(id);
            return StatusCode(result.Status, result);
        }
        [HttpGet("GetByAccount/{accountId}")]
        public async Task<IActionResult> GetByAccount(Guid accountId)
        {
            var result = await _service.GetByAccountIdAsync(accountId);
            return StatusCode(result.Status, result);
        }

        [HttpGet("GetByStaff/{staffId}")]
        public async Task<IActionResult> GetByStaff(Guid staffId)
        {
            var result = await _service.GetByStaffIdAsync(staffId);
            return StatusCode(result.Status, result);
        }
    }
}
