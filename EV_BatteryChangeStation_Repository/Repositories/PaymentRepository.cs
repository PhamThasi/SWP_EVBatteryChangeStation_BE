using EV_BatteryChangeStation_Repository.Base;
using EV_BatteryChangeStation_Repository.Entities;
using EV_BatteryChangeStation_Repository.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Repository.Repositories
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository() { }

        public PaymentRepository(EVBatterySwapContext context) => _context = context;
        public async Task<List<Payment>> GetAllPaymentDetailAsync()
        {
            return await _context.Payments
                .Include(s => s.Transaction)
                .Include(s => s.Subscription)
                .ToListAsync();
        }

        public async Task<List<Payment>> GetPaymentHistoryByAccountIdAsync(Guid accountId)
        {
            return await _context.Payments
                .Include(p => p.Subscription)
                    .ThenInclude(s => s.Account)
                .Include(p => p.Transaction)
                    .ThenInclude(t => t.Vehicle)
                .Include(p => p.Transaction)
                    .ThenInclude(t => t.Staff)
                .Include(p => p.Account)
                .Where(p =>
                    p.AccountId == accountId ||
                    (p.Subscription != null && p.Subscription.AccountId == accountId) ||
                    (p.Transaction != null &&
                     _context.Bookings.Any(b => b.VehicleId == p.Transaction.VehicleId && b.AccountId == accountId))
                )
                .OrderByDescending(p => p.CreateDate)
                .ToListAsync();
        }


        public async Task<Payment?> GetPaymentWithTransactionIdAsync(Guid transactionId)
        {
            return await _context.Payments
                .Include(p => p.Transaction)
                .FirstOrDefaultAsync(p => p.TransactionId.Equals(transactionId));
        }

        public async Task<Payment?> GetByGatewayIdAsync(long gatewayId)
        {
            return await _context.Payments
                .Include(p => p.Transaction)
                .Include(p => p.Subscription)
                .FirstOrDefaultAsync(t => t.PaymentGateId == gatewayId);
        }

        public async Task<bool> CheckPaymentOwnerAsync(Guid accountId, Payment payment)
        {
            if (payment == null || accountId == Guid.Empty)
                return false;

            bool isOwner = false;

            // 🧾 1️⃣ Kiểm tra Payment theo Subscription
            if (payment.SubscriptionId.HasValue)
            {
                isOwner = await _context.Subscriptions
                    .AnyAsync(s => s.SubscriptionId == payment.SubscriptionId && s.AccountId == accountId);
            }

            // 🚗 2️⃣ Kiểm tra Payment theo Transaction → Vehicle → Booking
            if (!isOwner && payment.TransactionId.HasValue)
            {
                var trans = await _context.SwappingTransactions
                    .Include(t => t.Vehicle)
                    .FirstOrDefaultAsync(t => t.TransactionId == payment.TransactionId);

                if (trans?.VehicleId != null && trans.VehicleId != Guid.Empty)
                {
                    isOwner = await _context.Bookings
                        .AnyAsync(b => b.VehicleId == trans.VehicleId && b.AccountId == accountId);
                }
            }

            return isOwner;
        }

        /// <summary>
        /// Lấy payment thành công có subscription còn hạn của account
        /// Dùng để check xem user có cần redirect đến trang thanh toán hay không
        /// </summary>
        public async Task<Payment?> GetActiveSubscriptionPaymentByAccountIdAsync(Guid accountId)
        {
            var now = DateTime.Now;

            return await _context.Payments
                .Include(p => p.Subscription)
                .Where(p =>
                    p.Status == "Successful" &&
                    p.SubscriptionId.HasValue &&
                    p.Subscription != null &&
                    (p.AccountId == accountId || p.Subscription.AccountId == accountId) &&
                    p.Subscription.IsActive == true &&
                    (p.Subscription.StartDate == null || p.Subscription.StartDate <= now) &&
                    (p.Subscription.EndDate == null || p.Subscription.EndDate >= now) &&
                    (!p.Subscription.RemainingSwaps.HasValue || p.Subscription.RemainingSwaps > 0)
                )
                .OrderByDescending(p => p.CreateDate)
                .FirstOrDefaultAsync();
        }

    }
}
