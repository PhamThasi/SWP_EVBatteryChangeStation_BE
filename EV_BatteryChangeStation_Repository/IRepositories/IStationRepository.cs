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
        Task<IEnumerable<Station>> GetAllAsync();
        Task<Station> GetByIdAsync(int id);
        void Create(Station station);
        void Update(Station station);
        void Delete(Station station);
    }
}
