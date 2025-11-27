using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Common.DTOs.PaymentDTO
{
    public class CreatePaymentDto
    {
        public decimal? Price { get; set; }
        public string Method { get; set; } = null;
        public long PaymentGateId { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? SubscriptionId { get; set; }
        public Guid? TransactionId { get; set; }
        /// <summary>
        /// AccountId của user mua subscription (bắt buộc nếu SubscriptionId có giá trị)
        /// </summary>
        public Guid? AccountId { get; set; }
    }
}
