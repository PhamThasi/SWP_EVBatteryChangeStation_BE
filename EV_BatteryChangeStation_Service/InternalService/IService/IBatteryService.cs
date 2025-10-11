using EV_BatteryChangeStation_Common.DTOs.BatteryDTO;
using EV_BatteryChangeStation_Repository.Base;
using EV_BatteryChangeStation_Repository.Entities;
using EV_BatteryChangeStation_Service.Base;
using Microsoft.AspNetCore.DataProtection.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Service.InternalService.IService
{
    public interface IBatteryService
    {
        Task<IServiceResult> IsBatteryAvailable(string batteryId);
        Task<IServiceResult> GetAllBattery();
        Task<IServiceResult> GetBatteryById(string batteryId);
        Task<IServiceResult> UpdateBatteryAsync(UpdateBattery updateDTO);
        Task<IServiceResult> GetAllBatteryByStationId(int stationId);
        Task<IServiceResult> CreateBatteryAsync(CreateBatteryDTO createBattery);
        Task<IServiceResult> GetBatteryCountByStationId(int stationId);
        Task<IServiceResult> DeleteBattery(string batteryId);
        Task<IServiceResult> SoftDeleteBaterry(string BatteryId);
    }
}
