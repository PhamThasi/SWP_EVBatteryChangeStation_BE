using EV_BatteryChangeStation_Common.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Common.Helper
{
    public class MailHelper
    {
        public static async Task<AutoEmailDTO> CreateResetPasswordOTPMail(string contact, string OTP, int? expiry = 5, string? lang = "vi-VN")
        {
            return new AutoEmailDTO()
            {
                RecipientEmail = contact,
                Subject = "Yêu cầu đặt lại mật khẩu",
                Body = $@"
                    Bạn vừa yêu cầu đặt lại mật khẩu cho tài khoản của mình.
                    Vui lòng sử dụng mã OTP dưới đây để hoàn tất quá trình đặt lại mật khẩu:
                    <div style='text-align: center;'>
                    <h2 style='color: #007bff; font-weight: bold;'>{OTP}</h2>
                    </div>
                    Mã OTP này sẽ hết hạn sau {expiry} phút.
                    [đây là tin nhắn tự động, vui lòng không phản hồi]",
            };
        }
    }
}
