using EV_BatteryChangeStation_Common.DTOs.PaymentDTO;
using EV_BatteryChangeStation_Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VNPAY.NET.Models;

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
                PaymentGateId = payment.PaymentGateId,
                CreateDate = payment.CreateDate,
                SubscriptionId = payment.SubscriptionId,
                TransactionId = payment.TransactionId,
                AccountId = payment.AccountId
            };
        }

        public static Payment toPayment(this CreatePaymentDto dto)
        {
            if (dto == null) return new Payment();
            return new Payment
            {
                PaymentId = Guid.NewGuid(), // Tạo PaymentId mới
                Price = dto.Price,
                Method = dto.Method,
                PaymentGateId = dto.PaymentGateId != 0 ? dto.PaymentGateId : DateTime.UtcNow.Ticks,
                CreateDate = dto.CreateDate ?? DateTime.UtcNow,
                SubscriptionId = dto.SubscriptionId,
                // Chỉ set TransactionId nếu có giá trị (tránh conflict với UNIQUE constraint khi NULL)
                TransactionId = dto.TransactionId.HasValue && dto.TransactionId.Value != Guid.Empty ? dto.TransactionId : null,
                AccountId = dto.AccountId
            };
        }

        public static void  UpdateToPayment(this Payment pay, UpdatePaymentDto dto)
        {
            if (dto == null || pay == null) return;
            
            if (!string.IsNullOrEmpty(dto.Status))
            {
                pay.Status = dto.Status;
            }
        }
        
        public static void UpdateToPaymentVNPay(this Payment pay, PaymentResult response)
        {
            pay.Status = response.IsSuccess ? "Successful" : "Failed";
            pay.CreateDate = DateTime.UtcNow;
            pay.Method = "VNPay";
        }
    }
}
