using EV_BatteryChangeStation_Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Repository.IRepositories
{
    public interface IStationRepository
    {
        Task<List<Station>> GetAllAsync();
        Task<Station> GetByIdAsync(Guid id);
        Task<List<Station>> SearchByNameAsync(string keyword);
        Task<Station> GetByNameAsync(string name);
        void Create(Station station);
        void Update(Station station);
        void Delete(Station station);
    }
}
