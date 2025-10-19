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
        Task<IServiceResult> GetCarByIdAsync(string carId);
        Task<IServiceResult> UpdateCarAsync(UpdateCarDto updateCarDto);
        Task<IServiceResult> DeleteCarAsync(string carId);
        Task<IServiceResult> SoftDeleteCarAsync(string carid);
        Task<IServiceResult> GetOwnerByCarIdAsync(string carid);
        Task<IServiceResult> GetCarByNameAsync(string modelName);
    }
}
