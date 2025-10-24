
using EV_BatteryChangeStation_Common.DTOs.BatteryDTO;
using EV_BatteryChangeStation_Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Repository.Mapper
{
    public static class BatteryMapper
    {
        public static ViewBatteryDTO MapToEntity(this Battery battery)
        {
            if (battery == null) throw new ArgumentNullException(nameof(battery), "cannot be null");
            return new ViewBatteryDTO
            {
                BatteryId = battery.BatteryId,
                Capacity = battery.Capacity,
                LastUsed = battery.LastUsed,
                Status = battery.Status,
                StateOfHealth = battery.StateOfHealth,
                PercentUse = battery.PercentUse,
                TypeBattery = battery.TypeBattery,
                BatterySwapDate = battery.BatterySwapDate,
                InsuranceDate = battery.InsuranceDate,
                StationId = battery.StationId
            };
        }

        public static Battery MaptoCreate(this CreateBatteryDTO batteryCreate)
        {
            if (batteryCreate == null) throw new ArgumentNullException(nameof(batteryCreate), "cannot be null");
            return new Battery
            {
                Capacity = batteryCreate.Capacity,
                Status = batteryCreate.Status,
                StateOfHealth = batteryCreate.StateOfHealth = 100,
                PercentUse = batteryCreate.PercentUse = 0,
                TypeBattery = batteryCreate.TypeBattery,
                InsuranceDate = batteryCreate.InsuranceDate,
                StationId = batteryCreate.StationId
            };
        }

        public static void MaptoUpdate(this Battery battery, UpdateBattery batteryUpdate)
        {
            if (battery == null) throw new ArgumentNullException(nameof(battery), "cannot be null");
            if (batteryUpdate == null) throw new ArgumentNullException(nameof(batteryUpdate), "cannot be null");
            if (batteryUpdate.Capacity.HasValue)
            {
                battery.Capacity = batteryUpdate.Capacity;
            }
            if (batteryUpdate.LastUsed.HasValue)
            {
                battery.LastUsed = batteryUpdate.LastUsed;
            }
            if (batteryUpdate.Status.HasValue)
            {
                battery.Status = batteryUpdate.Status;
            }
            if (batteryUpdate.StateOfHealth.HasValue)
            {
                battery.StateOfHealth = batteryUpdate.StateOfHealth;
            }
            if (batteryUpdate.PercentUse.HasValue)
            {
                battery.PercentUse = batteryUpdate.PercentUse;
            }
            if (!string.IsNullOrEmpty(batteryUpdate.TypeBattery))
            {
                battery.TypeBattery = batteryUpdate.TypeBattery;
            }
            if (batteryUpdate.BatterySwapDate.HasValue)
            {
                battery.BatterySwapDate = batteryUpdate.BatterySwapDate;
            }
            if (batteryUpdate.InsuranceDate.HasValue)
            {
                battery.InsuranceDate = batteryUpdate.InsuranceDate;
            }
        }
    }
}
