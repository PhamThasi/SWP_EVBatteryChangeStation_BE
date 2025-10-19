using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Common.DTOs.PaymentDTO
{
    public class ValidatePaymentDto
    {
        public string PaymentId { get; set; }
        [Required]
        public string? TransactionId { get; set; }
        [Required]
        public string? SubcriptionId { get; set; }
    }
}
