using EV_BatteryChangeStation_Common.DTOs.AuthencationDTO;
using EV_BatteryChangeStation_Common.DTOs.RegisterDTO;
using EV_BatteryChangeStation_Service.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Service.InternalService.IService
{
    public interface IAuthenService
    {
        public Task<IServiceResult> AuthenticationLogin(LoginDTO login);
        Task<bool> RegisterAsync(RegisterDTO dto);
        Task<string> SendOtpAsync(string email);
        Task<bool> VerifyOtpAsync(VerifyOtpDTO dto);
        Task<IServiceResult> LogoutAsync(string token);
        bool IsTokenRevoked(string token);
        Task<IServiceResult> ForgotPasswordSendOtpAsync(ForgotPasswordRequestDTO dto);
        Task<IServiceResult> VerifyForgotPasswordOtpAsync(VerifyForgotOtpDTO dto);
        Task<IServiceResult> ResetPasswordAsync(ResetPasswordDTO dto);
    }
}
