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

        ///<summary>
        ///lấy tất cả tài khoản với mã id được mã hóa
        ///</summary>
        /// <returns>Thông tin account lấy lên với id được mã hóa </returns>
        /// <response code="200">Lấy account thành công</response>
        /// <response code="400">Dữ liệu đầu vào không hợp lệ</response>
        /// <response code="500">Lỗi server khi xử lý yêu cầu</response>
        //[HttpGet("GetAllWithIdEncode")]
        //public async Task<IActionResult> GetAllWithIdEncode()
        //{
        //    var result = await _accountService.GetAllAccountWithIdDecodeAsync();
        //    if (result.Status == 200)
        //        return Ok(result);
        //    return StatusCode(result.Status, result.Message);
        //}

        ///<summary>
        ///Lấy tài khoản dựa trên tên
        /// </summary>
        /// <returns>Thông tin account lấy lên dựa trên tên tài khoản </returns>
        /// <response code="200">Lấy account thành công</response>
        /// <response code="400">Dữ liệu đầu vào không hợp lệ</response>
        /// <response code="500">Lỗi server khi xử lý yêu cầu</response>

        [HttpGet("GetAccountByName")]
        public async Task<IActionResult> GetAccountByName([FromQuery] string accview)
        {
            if (accview == null)
                return BadRequest("Get data fail");
            var result = await _accountService.GetAccountByNameAsync(accview);
            if (result.Status == 200)
                return Ok(result);
            return StatusCode(result.Status, result.Message);
        }

        ///<summary>
        ///Update tài khoản
        /// </summary>
        /// <returns>Update account </returns>
        /// <response code="200">Lấy account thành công</response>
        /// <response code="400">Dữ liệu đầu vào không hợp lệ</response>
        /// <response code="500">Lỗi server khi xử lý yêu cầu</response>

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] UpdateAccountDTO update)
        {
            if (update == null)
                return BadRequest("Get Data fail");
            var result = await _accountService.UpdateAccountAsync(update);
            if (result.Status == 200)
                return Ok(result);
            return StatusCode(result.Status, result.Message);
        }

        ///<summary>
        ///Xóa tài khoản
        /// </summary>
        /// <returns>Delete account </returns>
        /// <response code="200">Lấy account thành công</response>
        /// <response code="400">Dữ liệu đầu vào không hợp lệ</response>
        /// <response code="500">Lỗi server khi xử lý yêu cầu</response>

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete([FromQuery] Guid encode)
        {
            if (encode == Guid.Empty)
                return BadRequest("Gat Data fail");
            var result = await _accountService.DeleteAccountAsync(encode);
            if (result.Status == 200)
                return Ok(result);
            return StatusCode(result.Status, result.Message);
        }

        ///<summary>
        ///Xóa tài khoản bằng cách thay đổi trạng thái
        /// </summary>
        /// <returns>SoftDelete account </returns>
        /// <response code="200">Soft delete account thành công</response>
        /// <response code="400">Dữ liệu đầu vào không hợp lệ</response>
        /// <response code="500">Lỗi server khi xử lý yêu cầu</response>

        [HttpDelete("SoftDelete")]
        public async Task<IActionResult> SoftDelete([FromQuery] Guid encode)
        {
            if (encode == Guid.Empty)
                return BadRequest("Get Data fail");
            var result = await _accountService.SoftDeleteAsync(encode);
            if (result.Status == 200)
                return Ok(result);
            return StatusCode(result.Status, result.Message);
        }


        [HttpGet("GetAllStaffAccount")]
        public async Task<IActionResult> GetAllStaffAccount()
        {
            var result = await _accountService.GetAllStaffAccountAsync();
            if (result.Status == 200)
                return Ok(result);
            return StatusCode(result.Status, result.Message);
        }
    }
}
