using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Common.DTOs.AuthencationDTO
{
    public class ForgotPasswordRequestDTO
    {
        public string Email { get; set; }
    }

    public class VerifyForgotOtpDTO
    {
        public string Email { get; set; }
        public string OtpCode { get; set; }
    }

    public class ResetPasswordDTO
    {
        public string Email { get; set; }
        public string NewPassword { get; set; }
    }
}
