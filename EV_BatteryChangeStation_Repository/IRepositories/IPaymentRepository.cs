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
        Task<List<Payment?>> GetPaymentByAccountIdAsync(Guid accountId);
        Task<Payment?> GetByGatewayIdAsync(long gatewayId);
    }
}
