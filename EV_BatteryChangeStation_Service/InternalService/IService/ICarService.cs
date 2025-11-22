using EV_BatteryChangeStation_Common.DTOs.CarDTO;
using EV_BatteryChangeStation_Service.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Service.InternalService.IService
{
    public interface ICarService
    {
        Task<IServiceResult> AddCarAsync(CreateCarDto createCar);
        Task<IServiceResult> GetAllCarsAsync();
        Task<IServiceResult> GetCarByIdAsync(Guid carId);
        Task<IServiceResult> UpdateCarAsync(UpdateCarDto updateCarDto);
        Task<IServiceResult> DeleteCarAsync(Guid carId);
        Task<IServiceResult> SoftDeleteCarAsync(Guid carid);
        Task<IServiceResult> GetOwnerByCarIdAsync(Guid carid);
        Task<IServiceResult> GetCarByNameAsync(string modelName);
        Task<IServiceResult> GetCarsByOwnerIdAsync(Guid ownerId);
        Task<IServiceResult> GetBatteriesByCarAsync(Guid vehicle);
    }
}
