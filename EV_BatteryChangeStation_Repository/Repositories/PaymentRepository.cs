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

        public async Task<List<Payment?>> GetPaymentByAccountIdAsync(int accountId)
        {
            return await _context.Payments
                .Include(p => p.Subscription)
                .ThenInclude(s => s.Account)
                .Include(p => p.Transaction)
                .Where(p => p.Subscription.AccountId.Equals(accountId))
                .ToListAsync();
        }

        public async Task<Payment?> GetPaymentWithTransactionIdAsync(int transactionId)
        {
            return await _context.Payments
                .Include(p => p.Transaction)
                .FirstOrDefaultAsync(p => p.TransactionId.Equals(transactionId));
        }
    }
}
