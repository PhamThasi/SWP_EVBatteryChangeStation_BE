using EV_BatteryChangeStation_Repository.Base;
using EV_BatteryChangeStation_Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Repository.IRepositories
{
    public interface IBatteryRepository : IGenericRepository<Battery>
    {
        Task<int?> GetBatteryCountByStationId(int stationId);
        Task<bool?> IsBatteryAvailable(int batteryId);
        Task<List<Battery>> GetBatteryByStationId(int stationId); 
        Task<List<Battery>> GetAllBattery();
        Task<List<Battery?>> GetBatteriesByType(string typeBattery);
    }
}
