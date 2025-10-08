using EV_BatteryChangeStation_Service.Base;
using Microsoft.AspNetCore.DataProtection.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Service.InternalService.IService
{
    public interface IBattery
    {
        Task<IServiceResult> IsBatteryAvailable(string batteryId);
        Task<IServiceResult> GetAllBattery();
        Task<IServiceResult> GetBatteryById(string batteryId);
        Task<IServiceResult> UpdateBatteryStatus(string batteryId, string status);
        Task<IServiceResult> GetAllBatteryByStationId(string stationId);
        Task<IServiceResult> CreateBatteryAsync();
        Task<IServiceResult> GetBatteryCountByStationId(string stationId);
        Task<IServiceResult> DeleteBattery();
        Task<IServiceResult> SoftDeleteBaterry();
    }
}
