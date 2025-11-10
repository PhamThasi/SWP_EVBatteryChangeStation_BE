using EV_BatteryChangeStation_Common.DTOs.CarDTO;
using EV_BatteryChangeStation_Common.Enum.CarEnum;
using EV_BatteryChangeStation_Common.Enum.ServiceResult;
using EV_BatteryChangeStation_Repository.Entities;
using EV_BatteryChangeStation_Repository.Mapper;
using EV_BatteryChangeStation_Repository.UnitOfWork;
using EV_BatteryChangeStation_Service.Base;
using EV_BatteryChangeStation_Service.InternalService.IService;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Service.InternalService.Service
{
    public class CarService : ICarService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CarService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentException(nameof(unitOfWork));
        }
        // Add new car
        private bool IsValidUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return false;

            return Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        public async Task<IServiceResult> AddCarAsync(CreateCarDto createCar)
        {
            try
            {
                if (createCar == null)
                {
                    return new ServiceResult
                    {
                        Status = Const.ERROR_VALIDATION_CODE,
                        Message = Const.ERROR_INVALID_DATA_MSG,
                    };
                }
                if (!string.IsNullOrEmpty(createCar.Images) && !IsValidUrl(createCar.Images))
                {
                    return new ServiceResult
                    {
                        Status = Const.ERROR_VALIDATION_CODE,
                        Message = "Image URL is invalid. Please provide a valid HTTP or HTTPS URL.",
                    };
                }
                var result = createCar.MaptoCreate();
                await _unitOfWork.CarRepository.CreateAsync(result);
                return new ServiceResult
                {
                    Status = Const.SUCCESS_CREATE_CODE,
                    Message = Const.SUCCESS_CREATE_MSG,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult
                {
                    Status = Const.ERROR_EXCEPTION,
                    Message = ex.Message,
                };
            }
        }

        public async Task<IServiceResult> DeleteCarAsync(Guid carId)
        {
            try
            {
                if (carId == Guid.Empty)
                {
                    return new ServiceResult
                    {
                        Status = Const.ERROR_VALIDATION_CODE,
                        Message = Const.ERROR_INVALID_DATA_MSG,
                    };
                }
                var decodedId = await _unitOfWork.CarRepository.GetByIdAsync(carId);
                if (decodedId == null)
                {
                    return new ServiceResult
                    {
                        Status = Const.WARNING_NO_DATA_CODE,
                        Message = Const.WARNING_NO_DATA_MSG,
                    };
                }
                await _unitOfWork.CarRepository.RemoveAsync(decodedId);
                return new ServiceResult
                {
                    Status = Const.SUCCESS_DELETE_CODE,
                    Message = Const.SUCCESS_DELETE_MSG,
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult
                {
                    Status = Const.ERROR_EXCEPTION,
                    Message = ex.Message,
                };
            }
        }
        // Get all cars
        public async Task<IServiceResult> GetAllCarsAsync()
        {
            try
            {
                var result = await _unitOfWork.CarRepository.GetAllAsync();
                if (result == null)
                {
                    return new ServiceResult
                    {
                        Status = Const.WARNING_NO_DATA_CODE,
                        Message = Const.WARNING_NO_DATA_MSG,
                    };
                }
                var mappedResult = result.Select(car => car.MapToEntity()).ToList();
                return new ServiceResult
                {
                    Status = Const.SUCCESS_READ_CODE,
                    Message = Const.SUCCESS_READ_MSG,
                    Data = mappedResult
                };
            }
            catch(Exception ex)
            {
                return new ServiceResult
                {
                    Status = Const.ERROR_EXCEPTION,
                    Message = ex.Message,
                };
            }
        }

        // Get owner by car id
        public async Task<IServiceResult> GetOwnerByCarIdAsync(Guid carId)
        {
            try
            {
                // Kiểm tra đầu vào
                if (carId == Guid.Empty)
                {
                    return new ServiceResult
                    {
                        Status = Const.ERROR_VALIDATION_CODE,
                        Message = Const.ERROR_INVALID_DATA_MSG,
                    };
                }
                // Gọi repository
                var owner = await _unitOfWork.CarRepository.GetOwnerByCarIdAsync(carId);

                // Nếu không tìm thấy dữ liệu
                if (owner == null)
                {
                    return new ServiceResult
                    {
                        Status = Const.WARNING_NO_DATA_CODE,
                        Message = "Can not find the owner",
                    };
                }

                // Thành công
                return new ServiceResult
                {
                    Status = Const.SUCCESS_READ_CODE,
                    Message = Const.SUCCESS_READ_MSG,
                    Data = owner
                };
            }
            catch (Exception ex)
            {
                // Bắt lỗi
                return new ServiceResult
                {
                    Status = Const.ERROR_EXCEPTION,
                    Message = ex.Message,
                };
            }
        }

        // Get car by id
        public async Task<IServiceResult> GetCarByIdAsync(Guid carId)
        {
            try
            {
                if (carId == Guid.Empty)
                {
                    return new ServiceResult
                    {
                        Status = Const.ERROR_VALIDATION_CODE,
                        Message = Const.ERROR_INVALID_DATA_MSG,
                    };
                }
                var decodedId = await _unitOfWork.CarRepository.GetByIdAsync(carId);
                if (decodedId == null)
                {
                    return new ServiceResult
                    {
                        Status = Const.WARNING_NO_DATA_CODE,
                        Message = Const.WARNING_NO_DATA_MSG,
                    };
                }
                var mappedResult = decodedId.MapToEntity();
                return new ServiceResult
                {
                    Status = Const.SUCCESS_READ_CODE,
                    Message = Const.SUCCESS_READ_MSG,
                    Data = mappedResult
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult
                {
                    Status = Const.ERROR_EXCEPTION,
                    Message = ex.Message,
                };
            }
        }

        public async Task<IServiceResult> SoftDeleteCarAsync(Guid carid)
        {
            try
            {
                if (carid == Guid.Empty)
                {
                    return new ServiceResult
                    {
                        Status = Const.ERROR_VALIDATION_CODE,
                        Message = Const.ERROR_INVALID_DATA_MSG,
                    };
                }
                var car = await _unitOfWork.CarRepository.GetByIdAsync(carid);
                if (car == null)
                {
                    return new ServiceResult
                    {
                        Status = Const.WARNING_NO_DATA_CODE,
                        Message = Const.WARNING_NO_DATA_MSG,
                    };
                }
                car.Status = CarEnum.Unavailable.ToString();
                var result = await _unitOfWork.CarRepository.UpdateAsync(car);
                return new ServiceResult
                {
                    Status = Const.SUCCESS_DELETE_CODE,
                    Message = Const.SUCCESS_DELETE_MSG,
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult
                {
                    Status = Const.ERROR_EXCEPTION,
                    Message = ex.Message,
                };
            }
        }
        // Update car
        public async Task<IServiceResult> UpdateCarAsync(UpdateCarDto updateCarDto)
        {
            try
            {
                if (updateCarDto == null)
                {
                    return new ServiceResult
                    {
                        Status = Const.ERROR_VALIDATION_CODE,
                        Message = Const.ERROR_INVALID_DATA_MSG,
                    };
                }
                var car = await _unitOfWork.CarRepository.GetByIdAsync(updateCarDto.VehicleId);
                if (car == null)
                {
                    return new ServiceResult
                    {
                        Status = Const.WARNING_NO_DATA_CODE,
                        Message = Const.WARNING_NO_DATA_MSG,
                    };
                }

                if (!string.IsNullOrEmpty(updateCarDto.Images) && !IsValidUrl(updateCarDto.Images))
                {
                    return new ServiceResult
                    {
                        Status = Const.ERROR_VALIDATION_CODE,
                        Message = "Image URL is invalid. Please provide a valid HTTP or HTTPS URL.",
                    };
                }

                car.MaptoUpdate(updateCarDto);
                await _unitOfWork.CarRepository.UpdateAsync(car);
                return new ServiceResult
                {
                    Status = Const.SUCCESS_UPDATE_CODE,
                    Message = Const.SUCCESS_UPDATE_MSG,
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult
                {
                    Status = Const.ERROR_EXCEPTION,
                    Message = ex.Message,
                };
            }
        }

        public async Task<IServiceResult> GetCarByNameAsync(string modelName)
        {
            try
            {
                if (modelName.IsNullOrEmpty())
                {
                    return new ServiceResult
                    {
                        Status = Const.ERROR_VALIDATION_CODE,
                        Message = Const.ERROR_INVALID_DATA_MSG,
                    };
                }
                var result = await _unitOfWork.CarRepository.GetCarByNameAsync(modelName);
                if (result == null || result.Count == 0)
                {
                    return new ServiceResult
                    {
                        Status = Const.WARNING_NO_DATA_CODE,
                        Message = Const.WARNING_NO_DATA_MSG,
                    };
                }
                var mappedResult = result.Select(car => car.MapToEntity()).ToList();
                return new ServiceResult
                {
                    Status = Const.SUCCESS_READ_CODE,
                    Message = Const.SUCCESS_READ_MSG,
                    Data = mappedResult
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult
                {
                    Status = Const.ERROR_EXCEPTION,
                    Message = ex.Message,
                };
            }
        }

        public Task<IServiceResult> GetCarsByOwnerIdAsync(Guid ownerId)
        {
            try
            {
                if (ownerId == Guid.Empty)
                {
                    return Task.FromResult<IServiceResult>(new ServiceResult
                    {
                        Status = Const.ERROR_VALIDATION_CODE,
                        Message = Const.ERROR_INVALID_DATA_MSG,
                    });
                }
                var result = _unitOfWork.CarRepository.GetCarsByOwnerIdAsync(ownerId);
                if (result == null || result.Result.Count == 0)
                {
                    return Task.FromResult<IServiceResult>(new ServiceResult
                    {
                        Status = Const.WARNING_NO_DATA_CODE,
                        Message = Const.WARNING_NO_DATA_MSG,
                    });
                }
                var mappedResult = result.Result.Select(car => car?.MapToEntity()).ToList();
                return Task.FromResult<IServiceResult>(new ServiceResult
                {
                    Status = Const.SUCCESS_READ_CODE,
                    Message = Const.SUCCESS_READ_MSG,
                    Data = mappedResult
                });
            }
            catch (Exception ex)
            {
                return Task.FromResult<IServiceResult>(new ServiceResult
                {
                    Status = Const.ERROR_EXCEPTION,
                    Message = ex.Message,
                });
            }
        }

        public Task<IServiceResult> GetBatteriesByCarAsync(Guid vehicle)
        {
            try
            {
                if (vehicle == Guid.Empty)
                {
                    return Task.FromResult<IServiceResult>(new ServiceResult
                    {
                        Status = Const.ERROR_VALIDATION_CODE,
                        Message = Const.ERROR_INVALID_DATA_MSG,
                    });
                }
                var result = _unitOfWork.CarRepository.GetBatteriesByCarAsync(vehicle);
                if (result == null || result.Result.Count == 0)
                {
                    return Task.FromResult<IServiceResult>(new ServiceResult
                    {
                        Status = Const.WARNING_NO_DATA_CODE,
                        Message = Const.WARNING_NO_DATA_MSG,
                    });
                }
                return Task.FromResult<IServiceResult>(new ServiceResult
                {
                    Status = Const.SUCCESS_READ_CODE,
                    Message = Const.SUCCESS_READ_MSG,
                    Data = result.Result
                });
            }
            catch (Exception ex)
            {
                return Task.FromResult<IServiceResult>(new ServiceResult
                {
                    Status = Const.ERROR_EXCEPTION,
                    Message = ex.Message,
                });
            }
        }
    }
}
