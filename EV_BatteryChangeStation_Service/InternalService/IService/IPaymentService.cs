using EV_BatteryChangeStation_Common.DTOs.PaymentDTO;
using EV_BatteryChangeStation_Service.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Service.InternalService.IService
{
    public interface IPaymentService
    {
        Task<IServiceResult> CreatePayment(CreatePaymentDto create);
        Task<IServiceResult> GetPaymentById(Guid paymentId);
        Task<IServiceResult> UpdatePayment(UpdatePaymentDto update);
        Task<IServiceResult> DeletePayment(Guid paymentId);
        Task<IServiceResult> SoftDeletePayment(Guid paymentId);
        Task<IServiceResult> GetPaymentByAccountId(Guid accountId);
        Task<IServiceResult> GetAllPayment();
        Task<IServiceResult> GetPaymentByTransactionId(Guid transactionId);
        //Task<IServiceResult> ValidatePayment(ValidatePaymentDto validate);
        Task<IServiceResult> GetByGateWayId(long gateway);
        
        /// <summary>
        /// Check subscription status dựa vào payment để quyết định có cần redirect đến trang thanh toán hay không
        /// Nếu user đã có payment thành công với subscription active và còn hạn thì không cần redirect
        /// </summary>
        Task<IServiceResult> CheckSubscriptionStatus(Guid accountId);
    }
}
