using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Common.DTOs.PaymentDTO
{
    public class PaymentRespondDto
    {
        public Guid PaymentId { get; set; }
        public decimal? Price { get; set; }
        public string Method { get; set; }
        public string Status { get; set; }
        public long? PaymentGateId { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? SubscriptionId { get; set; }
        public Guid? TransactionId { get; set; }
        public Guid? AccountId { get; set; }
    }
}
