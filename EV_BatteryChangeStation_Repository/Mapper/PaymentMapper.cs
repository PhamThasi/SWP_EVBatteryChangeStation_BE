using EV_BatteryChangeStation_Common.DTOs.PaymentDTO;
using EV_BatteryChangeStation_Repository.Entities;
using HashidsNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Repository.Mapper
{
    public static class PaymentMapper
    {
        private static readonly Hashids _hashids = new Hashids("EV_BatteryChangeStation", 10); 
        public static PaymentRespondDto PaymentRespondDto(this Payment payment)
        {
            if (payment == null) return new PaymentRespondDto();
            return new PaymentRespondDto
            {
                PaymentId = _hashids.Encode(payment.PaymentId),
                Price = payment.Price,
                Method = payment.Method,
                Status = payment.Status,
                CreateDate = payment.CreateDate,
                SubscriptionId = payment.SubscriptionId,
                TransactionId = payment.TransactionId.HasValue
                    ? _hashids.Encode(payment.TransactionId.Value) : null
            };
        }

        public static Payment toPayment(this CreatePaymentDto dto, int? subscriptionId, int? transactionId)
        {
            if (dto == null) return new Payment();
            return new Payment
            {
                Price = dto.Price,
                Method = dto.Method,
                Status = dto.Status,
                CreateDate = DateTime.UtcNow,
                SubscriptionId = subscriptionId,
                TransactionId = transactionId
            };
        }

        public static void  UpdateToPayment(this Payment pay, UpdatePaymentDto dto)
        {
            if (dto == null || pay == null) return;
            
            if(dto.SubcriptionId.HasValue)
            {
                pay.SubscriptionId = dto.SubcriptionId;
            }
            if (!string.IsNullOrEmpty(dto.TransactionId))
            {
                pay.TransactionId = int.Parse(dto.TransactionId);
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
