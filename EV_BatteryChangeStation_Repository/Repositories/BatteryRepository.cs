using EV_BatteryChangeStation_Repository.Base;
using EV_BatteryChangeStation_Repository.Entities;
using EV_BatteryChangeStation_Repository.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Repository.Repositories
{
    public class BatteryRepository : GenericRepository<Battery>, IBatteryRepository
    {
        public BatteryRepository(){}
        public BatteryRepository(EVBatterySwapContext context) => _context = context;

        public Task<List<Battery>> GetAllBattery()
        {
            // Chỉ lấy pin còn khả dụng (Status = true)
            var battery = _context.Batteries
                .Where(b => b.Status == true)
                .ToListAsync();
            return battery;
        }

        public Task<List<Battery?>> GetBatteriesByType(string typeBattery)
        {
            // Chỉ lấy pin còn khả dụng (Status = true)
            return _context.Batteries
                .Where(b => b.TypeBattery.Contains(typeBattery) && b.Status == true)
                .ToListAsync();
        }

        //lấy tất cả pin trong trạm (chỉ lấy pin còn khả dụng)
        public async Task<List<Battery>> GetBatteryByStationId(Guid stationId)
        {
            var battery = await _context.Batteries
                .Where(b => b.StationId == stationId && b.Status == true)
                .ToListAsync();
            return battery;
        }

        //kiểm tra số lượng pin trong trạm
        public async Task<int?> GetBatteryCountByStationId(Guid stationId)
        {
            var station = await
                _context.Stations.Where(s => s.StationId == stationId)
                .Select(s => s.BatteryQuantity).FirstOrDefaultAsync();
            return station;
        }
        //Kiểm tra pin có thể được thay thế
        public async Task<bool?> IsBatteryAvailable(Guid batteryId)
        {
            var isAvailable = await _context.Batteries
                .AnyAsync(b => b.BatteryId == batteryId && b.Status == true && b.StateOfHealth > 80);
            return isAvailable;
        }

        public async Task<Battery?> GetAvailableBatteryAsync(Guid stationId, string typeBattery)
        {
            if (string.IsNullOrWhiteSpace(typeBattery))
            {
                return null;
            }

            return await _context.Batteries
                .Where(b => b.StationId == stationId
                            && b.TypeBattery != null
                            && b.Status == true
                            && b.StateOfHealth >= 80
                            && b.TypeBattery.ToLower() == typeBattery.ToLower())
                .OrderByDescending(b => b.StateOfHealth)
                .ThenBy(b => b.PercentUse)
                .FirstOrDefaultAsync();
        }
    }
}
