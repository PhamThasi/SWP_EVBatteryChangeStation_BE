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
    public class SwappingTransactionRepository : GenericRepository<SwappingTransaction>, ISwappingTransactionRepository
    {
        public SwappingTransactionRepository() { }
        public SwappingTransactionRepository(EVBatterySwapContext context) => _context = context;

        public async Task<List<SwappingTransaction>> GetAllSwappingTransactionDetail()
        {
            return await _context.SwappingTransactions
                .Include(a => a.Staff)
                .ThenInclude(t => t.Role)
                .Include(b => b.NewBattery)
                .Include(c => c.Payment)
                .Include(d => d.Vehicle)
                .Where(st => st.Staff.Role.RoleName == "Staff")
                .ToListAsync();
        }

        public async Task<List<SwappingTransaction>> getByCarId(Guid carId)
        {
            var swap = await _context.SwappingTransactions
                .Include(carId => carId.Vehicle)
                .Where(st => st.VehicleId == carId)
                .ToListAsync();
            return swap;
        }

    }
}
