using EV_BatteryChangeStation_Repository.Base;
using EV_BatteryChangeStation_Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Repository.IRepositories
{
    public interface IPaymentRepository : IGenericRepository<Payment>
    {
        Task<List<Payment>> GetAllPaymentDetailAsync();
        Task<Payment?> GetPaymentWithTransactionIdAsync(Guid transactionId);
        Task<List<Payment?>> GetPaymentHistoryByAccountIdAsync(Guid accountId);
        Task<Payment?> GetByGatewayIdAsync(long gatewayId);
        Task<bool> CheckPaymentOwnerAsync(Guid accountId, Payment payment);
        
        /// <summary>
        /// Lấy payment thành công có subscription còn hạn của account
        /// Dùng để check xem user có cần redirect đến trang thanh toán hay không
        /// </summary>
        Task<Payment?> GetActiveSubscriptionPaymentByAccountIdAsync(Guid accountId);
    }
}
