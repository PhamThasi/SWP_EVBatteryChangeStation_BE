using EV_BatteryChangeStation_Common.DTOs.CarDTO;
using EV_BatteryChangeStation_Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Repository.Mapper
{
    public static class CarMapper
    {

        public static ViewCarDTO MapToEntity(this Entities.Car car)
        {
            if (car == null) throw new ArgumentNullException(nameof(car), "cannot be null");
            return new ViewCarDTO
            {
                VehicleId = car.VehicleId,
                Model = car.Model,
                BatteryType = car.BatteryType,
                Producer = car.Producer,
                Images = car.Images,
                CreateDate = car.CreateDate,
                Status = car.Status
            };
        }

        public static Car MaptoCreate(this CreateCarDto carCreate)
        {
            if (carCreate == null) throw new ArgumentNullException(nameof(carCreate), "cannot be null");
            return new Entities.Car
            {
                Model = carCreate.Model,
                BatteryType = carCreate.BatteryType,
                Producer = carCreate.Producer,
                Images = carCreate.Images,
                CreateDate = carCreate.CreateDate,
                Status = carCreate.Status
            };
        }

        public static void MaptoUpdate(this Car car, UpdateCarDto carUpdate)
        {
            if (car == null) throw new ArgumentNullException(nameof(car), "cannot be null");
            if (carUpdate == null) throw new ArgumentNullException(nameof(carUpdate), "cannot be null");
            if (!string.IsNullOrEmpty(carUpdate.Model))
            {
                car.Model = carUpdate.Model;
            }
            if (!string.IsNullOrEmpty(carUpdate.BatteryType))
            {
                car.BatteryType = carUpdate.BatteryType;
            }
            if (!string.IsNullOrEmpty(carUpdate.Producer))
            {
                car.Producer = carUpdate.Producer;
            }
            if (!string.IsNullOrEmpty(carUpdate.Images))
            {
                car.Images = carUpdate.Images;
            }
            if (carUpdate.CreateDate.HasValue)
            {
                car.CreateDate = carUpdate.CreateDate;
            }
            if (!string.IsNullOrEmpty(carUpdate.Status))
            {
                car.Status = carUpdate.Status;
            }
        }
    }
}
