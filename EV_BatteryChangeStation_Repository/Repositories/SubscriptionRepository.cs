using EV_BatteryChangeStation_Repository.DBContext;
using EV_BatteryChangeStation_Repository.Entities;
using EV_BatteryChangeStation_Repository.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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

        public async Task<Subscription?> GetByIdAsync(int id)
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
    }
}
