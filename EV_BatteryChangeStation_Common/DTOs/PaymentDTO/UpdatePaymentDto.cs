using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Common.DTOs.PaymentDTO
{
    public class UpdatePaymentDto
    {
        public Guid PaymentId { get; set; }
        public Guid AccountId { get; set; }
        public string Status { get; set; }
    }
}
