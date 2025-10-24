using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Common.DTOs.PaymentDTO
{
    public class ViewPaymentDto
    {
        public Guid PaymentId { get; set; }
        public decimal? Price { get; set; }
        public string Method { get; set; }
        public bool? Status { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid SubscriptionId { get; set; }
        public Guid TransactionId { get; set; }
    }
}
