using EV_BatteryChangeStation_Common.DTOs.AuthencationDTO;
using EV_BatteryChangeStation_Common.DTOs.RegisterDTO;
using EV_BatteryChangeStation_Service.Base;
using EV_BatteryChangeStation_Service.InternalService.IService;
using Microsoft.AspNetCore.Authorization;
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
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            var result = await _authenService.RegisterAsync(dto);
            if (result) return Ok(new { message = "OTP sent to your email." });
            return BadRequest(new { message = "Email already exists." });
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp(VerifyOtpDTO dto)
        {
            var result = await _authenService.VerifyOtpAsync(dto);
            if (result) return Ok(new { message = "Registration successful." });
            return BadRequest(new { message = "Invalid OTP." });
        }
        /// <summary>
        /// Logout user (revoke JWT token)
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            // Lấy token từ header Authorization
            var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return BadRequest(new { message = "Token không hợp lệ" });
            }

            var token = authHeader.Replace("Bearer ", "");

            // Gọi service để logout (vd: xóa token khỏi DB hoặc blacklist)
            var result = await _authenService.LogoutAsync(token);

            // Trả về status code tương ứng
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Gửi OTP để đặt lại mật khẩu (Forgot Password)
        /// </summary>
        [HttpPost("forgot-password/send-otp")]
        public async Task<IActionResult> ForgotPasswordSendOtp([FromBody] ForgotPasswordRequestDTO dto)
        {
            var result = await _authenService.ForgotPasswordSendOtpAsync(dto);
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Xác thực OTP cho quên mật khẩu
        /// </summary>
        [HttpPost("forgot-password/verify-otp")]
        public async Task<IActionResult> VerifyForgotPasswordOtp([FromBody] VerifyForgotOtpDTO dto)
        {
            var result = await _authenService.VerifyForgotPasswordOtpAsync(dto);
            return StatusCode(result.Status, result);
        }


        /// <summary>
        /// Đặt lại mật khẩu sau khi xác thực OTP thành công
        /// </summary>
        [HttpPost("forgot-password/reset")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO dto)
        {
            var result = await _authenService.ResetPasswordAsync(dto);
            return StatusCode(result.Status, result);
        }
    }
}
