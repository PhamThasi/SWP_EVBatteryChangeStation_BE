using EV_BatteryChangeStation.Services;
using EV_BatteryChangeStation.ViewModels;
using EV_BatteryChangeStation_Repository.DBContext;
using EV_BatteryChangeStation_Repository.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace EV_BatteryChangeStation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private static Dictionary<string, RegisterVM> pendingUsers = new(); // Email - Register Info
        private static Dictionary<string, string> otpStorage = new(); // Email - OTP

        private readonly EmailService _emailService;
        private readonly EvbatterySwapContext _context; // DbContext để lưu vào DB

        public AuthController(EmailService emailService, EvbatterySwapContext context)
        {
            _emailService = emailService;
            _context = context;
        }

        // B1: Người dùng gửi thông tin đăng ký -> Hệ thống gửi OTP
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Tạo OTP 6 số
            var otp = new Random().Next(100000, 999999).ToString();

            // Lưu OTP + info tạm
            otpStorage[model.Email] = otp;
            pendingUsers[model.Email] = model;

            // Gửi email
            await _emailService.SendEmailAsync(model.Email, "Your OTP Code", $"Your OTP is: <b>{otp}</b>");

            return Ok(new { message = "OTP sent to email. Please confirm OTP to complete registration." });
        }

        // B2: Xác thực OTP
        [HttpPost("confirm-otp")]
        public IActionResult ConfirmOTP([FromBody] ConfirmOTPVM model)
        {
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.OTP))
                return BadRequest(new { message = "Email and OTP are required." });

            if (otpStorage.ContainsKey(model.Email) && otpStorage[model.Email] == model.OTP)
            {
                if (!pendingUsers.ContainsKey(model.Email))
                    return BadRequest(new { message = "No pending registration for this email." });

                var userData = pendingUsers[model.Email];

                // 👉 Lấy RoleID hợp lệ từ DB (ví dụ mặc định là User)
                var defaultRole = _context.Roles.FirstOrDefault(r => r.RoleName == "User");
                if (defaultRole == null)
                    return StatusCode(500, new { message = "Default role not found. Please seed Roles first." });

                // 👉 Hash password trước khi lưu
                string hashedPassword = HashPassword(userData.Password);

                // 👉 Lưu Account vào database
                var account = new Account
                {
                    AccountName = userData.AccountName,
                    FullName = userData.FullName,
                    Password = hashedPassword,
                    Email = userData.Email,
                    Gender = userData.Gender,
                    Address = userData.Address,
                    PhoneNumber = userData.PhoneNumber,
                    DateOfBirth = userData.DateOfBirth,
                    RoleId = defaultRole.RoleId, // ✅ gán RoleID hợp lệ
                    Status = true // nếu Account có cột Status
                };

                _context.Accounts.Add(account);
                _context.SaveChanges();

                // Xóa OTP & pending info
                otpStorage.Remove(model.Email);
                pendingUsers.Remove(model.Email);

                return Ok(new { message = "Registration successful." });
            }

            return BadRequest(new { message = "Invalid OTP." });
        }

        // Hàm hash password
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
