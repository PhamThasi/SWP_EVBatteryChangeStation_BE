using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Common.DTOs.RegisterDTO
{
    // DTO xác nhận OTP
    public class VerifyOtpDTO
    {
        [Required]
        public string Email { get; set; }


        [Required]
        public string OtpCode { get; set; }
    }

}
