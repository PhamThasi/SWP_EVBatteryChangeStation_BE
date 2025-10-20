using EV_BatteryChangeStation_Common.DTOs.PaymentDTO;
using EV_BatteryChangeStation_Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Repository.Mapper
{
    public static class PaymentMapper
    {
        public static PaymentRespondDto PaymentRespondDto(this Payment payment)
        {
            if (payment == null) return new PaymentRespondDto();
            return new PaymentRespondDto
            {
                PaymentId = payment.PaymentId,
                Price = payment.Price,
                Method = payment.Method,
                Status = payment.Status,
                CreateDate = payment.CreateDate,
                SubscriptionId = payment.SubscriptionId ?? Guid.Empty,
                TransactionId = payment.TransactionId ?? Guid.Empty
            };
        }

        public static Payment toPayment(this CreatePaymentDto dto)
        {
            if (dto == null) return new Payment();
            return new Payment
            {
                Price = dto.Price,
                Method = dto.Method,
                Status = dto.Status,
                CreateDate = DateTime.UtcNow,
                SubscriptionId = dto.SubscriptionId,
                TransactionId = dto.TransactionId
            };
        }

        public static void  UpdateToPayment(this Payment pay, UpdatePaymentDto dto)
        {
            if (dto == null || pay == null) return;
            
            if(dto.SubcriptionId != Guid.Empty)
            {
                pay.SubscriptionId = dto.SubcriptionId;
            }
            if (dto.TransactionId != Guid.Empty)
            {
                pay.TransactionId = dto.TransactionId;
            }
            if (dto.Price.HasValue)
            {
                pay.Price = dto.Price.Value;
            }
            if (!string.IsNullOrEmpty(dto.Method))
            {
                pay.Method = dto.Method;
            }
            if (dto.Status.HasValue)
            {
                pay.Status = dto.Status;
            }
        }
        
    }
}
