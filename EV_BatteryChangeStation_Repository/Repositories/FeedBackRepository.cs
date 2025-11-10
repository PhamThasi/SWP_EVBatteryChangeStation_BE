using EV_BatteryChangeStation_Repository.Entities;
using EV_BatteryChangeStation_Repository.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace EV_BatteryChangeStation_Repository.Repositories
{
    public class FeedBackRepository : IFeedBackRepository
    {
        private readonly EVBatterySwapContext _context;

        public FeedBackRepository(EVBatterySwapContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Feedback>> GetAllAsync()
        {
            return await _context.Feedbacks.ToListAsync();
        }

        public async Task<Feedback> GetByIdAsync(Guid id)
        {
            return await _context.Feedbacks.FirstOrDefaultAsync(f => f.FeedbackId == id);
        }

        public async Task AddAsync(Feedback feedback)
        {
            await _context.Feedbacks.AddAsync(feedback);
        }

        public void Update(Feedback feedback)
        {
            _context.Feedbacks.Update(feedback);
        }

        public void Delete(Feedback feedback)
        {
            _context.Feedbacks.Remove(feedback);
        }
        public async Task<IEnumerable<Feedback>> GetByAccountIdAsync(Guid accountId)
        {
            return await _context.Feedbacks
                .Where(f => f.AccountId == accountId)
                .ToListAsync();
        }

    }
}
