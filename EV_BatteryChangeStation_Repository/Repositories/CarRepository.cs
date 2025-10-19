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

        public Task<List<Car>> GetCarByNameAsync(string modelName)
        {
            return _context.Cars
                .Where(c => c.Model.Contains(modelName))
                .ToListAsync();
        }

        // Lấy chủ sở hữu xe theo VehicleId (bỏ HashIds)
        public async Task<Account?> GetOwnerByCarIdAsync(int carId)
        {
            var owner = await _context.Bookings
                .Include(b => b.Account)
                .Where(b => b.VehicleId == carId)
                .OrderByDescending(b => b.CreatedDate) // lấy booking mới nhất
                .Select(b => b.Account)
                .FirstOrDefaultAsync();

            return owner;
        }
    }
}
