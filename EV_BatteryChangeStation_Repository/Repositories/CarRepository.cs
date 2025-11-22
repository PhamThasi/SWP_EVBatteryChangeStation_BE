using EV_BatteryChangeStation_Repository.Base;
using EV_BatteryChangeStation_Repository.Entities;
using EV_BatteryChangeStation_Repository.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Repository.Repositories
{
    public class CarRepository : GenericRepository<Car>, ICarRepository
    {
        public CarRepository() { }

        public CarRepository(EVBatterySwapContext context)
        {
            _context = context;
        }

        public async Task<List<Battery>> GetBatteriesByCarAsync(Guid vehicleId)
        {
            // Lấy loại pin của xe
            var batteryType = await _context.Cars
                .Where(c => c.VehicleId == vehicleId)
                .Select(c => c.BatteryType)
                .FirstOrDefaultAsync();

            if (batteryType == null)
                return new List<Battery>(); // xe không tồn tại

            // Lấy tất cả pin cùng loại
            var batteries = await _context.Batteries
                .Where(b => b.TypeBattery == batteryType)
                .ToListAsync();

            return batteries;
        }


        public Task<List<Car>> GetCarByNameAsync(string modelName)
        {
            return _context.Cars
                .Where(c => c.Model.Contains(modelName))
                .ToListAsync();
        }

        public async Task<List<Car?>> GetCarsByOwnerIdAsync(Guid ownerId)
        {
            var cars = await _context.Bookings
                .Include(b => b.Vehicle)
                .Where(b => b.AccountId == ownerId)
                .Select(b => b.Vehicle)
                .Distinct()
                .ToListAsync();
            return cars;
        }

        public async Task<Account?> GetOwnerByCarIdAsync(Guid carId)
        {
            var owner = await _context.Bookings
                .Include(b => b.Account)
                .Where(b => b.VehicleId == carId)
                .OrderByDescending(b => b.CreatedDate) 
                .Select(b => b.Account)
                .FirstOrDefaultAsync();

            return owner;
        }

    }
}
