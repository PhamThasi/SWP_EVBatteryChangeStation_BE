using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Common.DTOs.VNPayDTO
{
    public class VNPayConfigDto
    {
        public string TmnCode { get; set; } = null!;
        public string HashSecret { get; set; } = null!;
        public string ReturnUrl { get; set; } = null!;
        public string PaymentUrl { get; set; } = null!;
    }
}
