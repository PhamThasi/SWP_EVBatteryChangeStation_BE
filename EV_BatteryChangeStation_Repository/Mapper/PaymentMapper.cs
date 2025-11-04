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
                TransactionId = payment.TransactionId
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
                PaymentGateId = DateTime.UtcNow.Ticks,
                CreateDate = DateTime.UtcNow,
                SubscriptionId = dto.SubscriptionId,
                TransactionId = dto.TransactionId
            };
        }

        public static void  UpdateToPayment(this Payment pay, UpdatePaymentDto dto)
        {
            if (dto == null || pay == null) return;
            
            if (dto.Status.HasValue)
            {
                pay.Status = dto.Status;
            }
        }
        
        public static void UpdateToPaymentVNPay(this Payment pay, PaymentResult response)
        {
            pay.Status = response.IsSuccess ? true : false;
            pay.CreateDate = DateTime.UtcNow;
            pay.Method = "VNPay";
        }
    }
}
