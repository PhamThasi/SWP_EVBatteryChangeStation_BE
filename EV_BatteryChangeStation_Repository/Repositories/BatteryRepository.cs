using EV_BatteryChangeStation_Repository.Base;
using EV_BatteryChangeStation_Repository.DBContext;
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
            var battery = _context.Batteries.ToListAsync();
            return battery;
        }

        //lấy tất cả pin trong trạm
        public async Task<List<Battery>> GetBatteryByStationId(int stationId)
        {
            var battery = await _context.Batteries
                .Where(b => b.StationId == stationId)
                .ToListAsync();
            return battery;
        }

        //kiểm tra số lượng pin trong trạm
        public async Task<int?> GetBatteryCountByStationId(int stationId)
        {
            var station = await
                _context.Stations.Where(s => s.StationId == stationId)
                .Select(s => s.BatteryQuantity).FirstOrDefaultAsync();
            return station;
        }
        //Kiểm tra pin có thể được thay thế
        public async Task<bool?> IsBatteryAvailable(int batteryId)
        {
            var isAvailable = await _context.Batteries
                .AnyAsync(b => b.BatteryId == batteryId && b.Status == true && b.StateOfHealth > 80);
            return isAvailable;
        }
    }
}
