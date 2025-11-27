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
        Task<int?> GetBatteryCountByStationId(Guid stationId);
        Task<bool?> IsBatteryAvailable(Guid batteryId);
        Task<List<Battery>> GetBatteryByStationId(Guid stationId); 
        Task<List<Battery>> GetAllBattery();
        Task<List<Battery?>> GetBatteriesByType(string typeBattery);
        Task<Battery?> GetAvailableBatteryAsync(Guid stationId, string typeBattery);
    }
}
