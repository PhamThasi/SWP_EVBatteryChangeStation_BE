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
        Task<IServiceResult> IsBatteryAvailable(Guid batteryId);
        Task<IServiceResult> GetAllBattery();
        Task<IServiceResult> GetBatteryById(Guid batteryId);
        Task<IServiceResult> UpdateBatteryAsync(UpdateBattery updateDTO);
        Task<IServiceResult> GetAllBatteryByStationId(Guid stationId);
        Task<IServiceResult> CreateBatteryAsync(CreateBatteryDTO createBattery);
        Task<IServiceResult> GetBatteryCountByStationId(Guid stationId);
        Task<IServiceResult> DeleteBattery(Guid batteryId);
        Task<IServiceResult> SoftDeleteBattery(Guid BatteryId);
        Task<IServiceResult> GetBatteriesByType(string typeBattery);
        Task<IServiceResult> GetBatteriesByStaffStationAsync(Guid staffAccountId);
        Task<IServiceResult> PreviewBatteryForBookingAsync(Guid stationId, Guid vehicleId);
    }
}
