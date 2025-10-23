using EV_BatteryChangeStation_Common.DTOs.AuthencationDTO;
using EV_BatteryChangeStation_Common.DTOs.RegisterDTO;
using EV_BatteryChangeStation_Common.Enum.ServiceResult;
using EV_BatteryChangeStation_Repository.Entities;
using EV_BatteryChangeStation_Repository.UnitOfWork;
using EV_BatteryChangeStation_Service.Base;
using EV_BatteryChangeStation_Service.ExternalService.IService;
using EV_BatteryChangeStation_Service.InternalService.IService;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Service.InternalService.Service
{
    public class AuthenService : IAuthenService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasher<Account> _passwordHasher;
        private static Dictionary<string, string> pendingOtps = new Dictionary<string, string>();
        private readonly IJWTService _jwtService;
        private static readonly List<string> _blacklistedTokens = new();
        private static Dictionary<string, RegisterDTO> pendingUsers = new Dictionary<string, RegisterDTO>();
        // khởi tạo các service cần thiết qua dependency injection
        public AuthenService(UnitOfWork unitOfWork, IPasswordHasher<Account> passwordHasher, IConfiguration configuration, IJWTService jwtservice)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _jwtService = jwtservice ?? throw new ArgumentNullException(nameof(jwtservice));
        }
        // đăng nhập
        public async Task<IServiceResult> AuthenticationLogin(LoginDTO login)
        {
            var account = await _unitOfWork.AccountRepository.GetByAccountNameOrEmail(login.Keyword.ToLower());
            if (account == null) // không tìm thấy tài khoản
            {
                return new ServiceResult
                {
                    Status = Const.WARNING_NO_DATA_CODE,
                    Message = Const.WARNING_NO_DATA_MSG,
                    Errors = new List<string> { "Account not found" }
                };
            }
            if (account.Status == false) // tài khoản bị khóa
            {
                return new ServiceResult
                {
                    Status = Const.FORBIDDEN_ACCESS_CODE,
                    Message = Const.FORBIDDEN_ACCESS_MSG,
                    Errors = new List<string> { "Your account has been baned. Please contact support for more infomation." }
                };
            }
            // kiểm tra mật khẩu
            var result = _passwordHasher.VerifyHashedPassword(account, account.Password, login.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                return new ServiceResult
                {
                    Status = Const.UNAUTHORIZED_ACCESS_CODE,
                    Message = Const.UNAUTHORIZED_ACCESS_MSG,
                    Errors = new List<string> { "Password is incorrect" }
                };
            }
            //lưu lại thông tin đăng nhập
            LoginRespondDTO tokenDto = new()
            {
                AccountId = account.AccountId,
                AccountName = account.AccountName,
                Email = account.Email,
                RoleName = account.Role?.RoleName
            };
            var token = _jwtService.GenerateToken(tokenDto);
            
            return new ServiceResult
            {
                Status = Const.SUCCESS_LOGIN_CODE,
                Message = Const.SUCCESS_LOGIN_MSG,
                Data = token
            };
        }

        public async Task<bool> RegisterAsync(RegisterDTO dto)
        {
            // kiểm tra email tồn tại chưa
            var existing = await _unitOfWork.AccountRepository.GetAccountByEmail(dto.Email);
            if (existing != null) return false;

            // tạo account nhưng chưa lưu, gửi OTP trước
            var otp = GenerateOtp();
            pendingOtps[dto.Email] = otp;
            pendingUsers[dto.Email] = dto;

            // gửi mail OTP
            await SendOtpEmail(dto.Email, otp);

            return true; // trả về true nghĩa là OTP đã gửi
        }

        public Task<string> SendOtpAsync(string email)
        {
            if (pendingOtps.TryGetValue(email, out var otp))
            {
                return Task.FromResult(otp);
            }
            return Task.FromResult<string>(null);
        }

        public async Task<bool> VerifyOtpAsync(VerifyOtpDTO dto)
        {
            if (pendingOtps.TryGetValue(dto.Email, out var otp) && otp == dto.OtpCode)
            {
                if (!pendingUsers.TryGetValue(dto.Email, out var registerDto))
                    return false; // không tìm thấy RegisterDTO tạm

                // Lấy role "Customer" từ database
                var customerRole = await _unitOfWork.RoleRepository.GetRoleByName("Customer");

                if (customerRole == null)
                    throw new Exception("Role 'Customer' not found in database.");

                // tạo account thật
                var account = new Account
                {
                    Email = registerDto.Email,
                    AccountName = registerDto.Email.Split('@')[0],
                    //FullName = registerDto.FullName,
                    Password = _passwordHasher.HashPassword(null, registerDto.Password),
                    RoleId = customerRole.RoleId, // Gán role "Customer" lấy từ DB
                    Status = true
                };

                _unitOfWork.AccountRepository.Create(account);
                await _unitOfWork.AccountRepository.SaveAsync();

                // xóa OTP và RegisterDTO tạm
                pendingOtps.Remove(dto.Email);
                pendingUsers.Remove(dto.Email);

                return true;
            }
            return false;
        }



        private string GenerateOtp()
        {
            var rnd = new Random();
            return rnd.Next(100000, 999999).ToString();
        }

        private Task SendOtpEmail(string email, string otp)
        {
            string fromEmail = _configuration["EmailSettings:Email"];
            string password = _configuration["EmailSettings:AppPassword"];

            using var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(fromEmail, password),
                EnableSsl = true
            };

            var mail = new MailMessage(fromEmail, email)
            {
                Subject = "Your OTP Code",
                Body = $"Your OTP code is: {otp}",
                IsBodyHtml = false
            };

            client.Send(mail);
            return Task.CompletedTask;
        }

        public Task<IServiceResult> LogoutAsync(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                _blacklistedTokens.Add(token);
            }

            return Task.FromResult<IServiceResult>(new ServiceResult
            {
                Status = 200,
                Message = "Logout success. Token has been eliminated."
            });
        }

        public bool IsTokenRevoked(string token)
        {
            return _blacklistedTokens.Contains(token);
        }
    }
}
