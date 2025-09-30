using EV_BatteryChangeStation_Common.DTOs.AuthencationDTO;
using EV_BatteryChangeStation_Service.Base;
using EV_BatteryChangeStation_Service.InternalService.IService;
using Microsoft.AspNetCore.Mvc;

namespace EV_BatteryChangeStation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenController : ControllerBase
    {
        private readonly IAuthenService _authenService;

        public AuthenController(IAuthenService authenService)
        {
            _authenService = authenService ?? throw new ArgumentNullException(nameof(authenService));
        }

        /// <summary>
        /// đăng nhập người dùng
        /// </summary>
        /// <param name="request">Tài khoản + mật khẩu</param>
        /// <returns code="200">đăng nhập thành công</returns>
        /// <returns code="400">data truyền vào trống</returns>
        /// <returns code="401">Sai mật khẩu</returns>
        /// <returns code="403">Tài khoản bị khóa</returns>
        /// <returns code="404">Không tìm thấy tài khoản</returns>
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            IServiceResult result = new ServiceResult();
            try
            {
                result = await _authenService.AuthenticationLogin(login);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(result.Status, new ServiceResult
                {
                    Status = result.Status,
                    Message = result.Message,
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}
