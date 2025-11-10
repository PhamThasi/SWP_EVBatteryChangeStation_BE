using EV_BatteryChangeStation_Repository.Base;
using EV_BatteryChangeStation_Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Repository.IRepositories
{
    public interface ICarRepository : IGenericRepository<Car>
    {
        Task<Account?> GetOwnerByCarIdAsync(Guid carId);
        Task<List<Car>> GetCarByNameAsync(string modelName);
        Task<List<Car?>> GetCarsByOwnerIdAsync(Guid ownerId);   
        Task<List<Battery>> GetBatteriesByCarAsync(Guid vehicleId);
    }
}
