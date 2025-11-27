
using EV_BatteryChangeStation_Repository.Entities;
using EV_BatteryChangeStation_Repository.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Repository.Repositories
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly EVBatterySwapContext _context;

        public SubscriptionRepository(EVBatterySwapContext context)
        {
            _context = context;
        }

        public async Task<List<Subscription>> GetAllAsync()
        {
            return await _context.Subscriptions.ToListAsync();
        }

        public async Task<Subscription?> GetByIdAsync(Guid id)
        {
            return await _context.Subscriptions.FirstOrDefaultAsync(x => x.SubscriptionId == id);
        }

        public async Task AddAsync(Subscription entity)
        {
            await _context.Subscriptions.AddAsync(entity);
        }

        public void Update(Subscription entity)
        {
            _context.Subscriptions.Update(entity);
        }

        public void Delete(Subscription entity)
        {
            _context.Subscriptions.Remove(entity);
        }

        /// <summary>
        /// Lấy gói đang hoạt động của một Account,
        /// thỏa điều kiện: IsActive = true, còn hạn (StartDate/EndDate)
        /// và còn lượt swap (RemainingSwaps > 0) nếu gói có giới hạn lượt.
        /// </summary>
        public async Task<Subscription?> GetActiveByAccountIdAsync(Guid accountId)
        {
            var now = DateTime.Now;

            return await _context.Subscriptions
                .FirstOrDefaultAsync(s =>
                    s.AccountId == accountId &&
                    s.IsActive == true &&
                    (s.StartDate == null || s.StartDate <= now) &&
                    (s.EndDate == null || s.EndDate >= now) &&
                    (!s.RemainingSwaps.HasValue || s.RemainingSwaps > 0));
        }
    }
}
