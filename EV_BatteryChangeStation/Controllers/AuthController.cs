using Microsoft.AspNetCore.Mvc;
using EV_BatteryChangeStation.ViewModels;
using EV_BatteryChangeStation.Services;

namespace EV_BatteryChangeStation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private static Dictionary<string, string> otpStorage = new(); // Email - OTP
        private readonly EmailService _emailService;

        public AuthController(EmailService emailService)
        {
            _emailService = emailService;
        }

        // B1: Người dùng gửi thông tin đăng ký -> Hệ thống gửi OTP
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Tạo OTP 6 số
            var otp = new Random().Next(100000, 999999).ToString();

            // Lưu OTP tạm theo email
            otpStorage[model.Email] = otp;

            // Gửi email
            await _emailService.SendEmailAsync(model.Email, "Your OTP Code", $"Your OTP is: <b>{otp}</b>");

            return Ok(new { message = "OTP sent to email. Please confirm OTP to complete registration." });
        }

        // B2: Xác thực OTP
        [HttpPost("confirm-otp")]
        public IActionResult ConfirmOTP([FromBody] RegisterVM model)
        {
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.OTP))
                return BadRequest(new { message = "Email and OTP are required." });

            if (otpStorage.ContainsKey(model.Email) && otpStorage[model.Email] == model.OTP)
            {
                // 👉 TODO: Lưu thông tin Account vào database ở đây
                otpStorage.Remove(model.Email);
                return Ok(new { message = "Registration successful." });
            }

            return BadRequest(new { message = "Invalid OTP." });
        }
    }
}
