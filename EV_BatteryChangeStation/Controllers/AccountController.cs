using EV_BatteryChangeStation_Common.DTOs.AccountDto;
using EV_BatteryChangeStation_Service.InternalService.IService;
using Microsoft.AspNetCore.Mvc;

namespace EV_BatteryChangeStation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
        }

        /// <summary>
        /// Tạo mới account
        /// </summary>
        /// <param name="dto">Thông tin account cần tạo</param>
        /// <returns>Thông tin account đã được tạo</returns>
        /// <response code="200">Tạo account thành công</response>
        /// <response code="400">Dữ liệu đầu vào không hợp lệ</response>
        /// <response code="500">Lỗi server khi xử lý yêu cầu</response>

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreateAccountDTO dto)
        {
            if (dto == null)
                return BadRequest("Invalid account data");
            var result = await _accountService.CreateAccountAsync(dto);
            if (result.Status == 200)
                return Ok(result);
            return StatusCode(result.Status, result.Message);
        }

        /// <summary>
        /// lấy tất cả account
        /// </summary>
        /// <param name="dto">Thông tin account</param>
        /// <returns>Thông tin account lấy  lên </returns>
        /// <response code="200">Lấy account thành công</response>
        /// <response code="400">Dữ liệu đầu vào không hợp lệ</response>
        /// <response code="500">Lỗi server khi xử lý yêu cầu</response>
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _accountService.GetAllAccountsAsync();
            if (result.Status == 200)
                return Ok(result);
            return StatusCode(result.Status, result.Message);
        }
    }
}
