using EV_BatteryChangeStation_Repository.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Repository.IRepositories
{
    public interface ISubscriptionRepository
    {
        Task<List<Subscription>> GetAllAsync();
        Task<Subscription?> GetByIdAsync(Guid id);
        Task AddAsync(Subscription entity);
        void Update(Subscription entity);
        void Delete(Subscription entity);
    }
}
