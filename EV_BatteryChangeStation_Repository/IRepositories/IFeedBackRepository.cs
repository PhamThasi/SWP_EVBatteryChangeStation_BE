using EV_BatteryChangeStation_Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Repository.IRepositories
{
    public interface IFeedBackRepository
    {
        Task<IEnumerable<Feedback>> GetAllAsync();
        Task<Feedback> GetByIdAsync(Guid id);
        Task AddAsync(Feedback feedback);
        void Update(Feedback feedback);
        void Delete(Feedback feedback);
        Task<IEnumerable<Feedback>> GetByAccountIdAsync(Guid accountId);

    }
}
