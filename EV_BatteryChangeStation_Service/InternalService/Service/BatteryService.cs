using EV_BatteryChangeStation_Common.DTOs.BatteryDTO;
using EV_BatteryChangeStation_Common.Enum.ServiceResult;
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
    public class BatteryService : IBatteryService
    {
        private readonly IUnitOfWork _unitOfWork;
        public BatteryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentException(nameof(unitOfWork));

        }
        // Tạo pin mới
        public async Task<IServiceResult> CreateBatteryAsync(CreateBatteryDTO createBattery)
        {
            try
            {
                if (createBattery == null)
                {
                    return new ServiceResult
                    {
                        Status = Const.ERROR_VALIDATION_CODE,
                        Message = Const.ERROR_INVALID_DATA_MSG,
                    };
                }
                var battery = createBattery.MaptoCreate();
                await _unitOfWork.BatteryRepository.CreateAsync(battery);
                return new ServiceResult
                {
                    Status = Const.SUCCESS_CREATE_CODE,
                    Message = Const.SUCCESS_CREATE_MSG,
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
        // Xoá cứng pin
        public async Task<IServiceResult> DeleteBattery(Guid batteryId)
        {
            try
            {
                if (batteryId == Guid.Empty)
                {
                    return new ServiceResult
                    {
                        Status = Const.ERROR_VALIDATION_CODE,
                        Message = Const.ERROR_INVALID_DATA_MSG,
                    };
                }
                var battery = await _unitOfWork.BatteryRepository.GetByIdAsync(batteryId);
                if (battery == null)
                {
                    return new ServiceResult
                    {
                        Status = Const.FAIL_READ_CODE,
                        Message = Const.FAIL_READ_MSG,
                    };
                }
                await _unitOfWork.BatteryRepository.RemoveAsync(battery);
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
        // Lấy tất cả pin
        public async Task<IServiceResult> GetAllBattery()
        {
            try
            {
                var battery = await _unitOfWork.BatteryRepository.GetAllBattery();
                if (battery == null || !battery.Any())
                {
                    return new ServiceResult
                    {
                        Status = Const.WARNING_NO_DATA_CODE,
                        Message = Const.WARNING_NO_DATA_MSG,
                    };
                }
                var batteryDtos = battery.Select(b => b.MapToEntity()).ToList();
                return new ServiceResult
                {
                    Status = Const.SUCCESS_READ_CODE,
                    Message = Const.SUCCESS_READ_MSG,
                    Data = batteryDtos
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
        //lấy tất cả pin trong trạm
        public async Task<IServiceResult> GetAllBatteryByStationId(Guid stationId)
        {
            try
            {
                if (stationId == Guid.Empty)
                {
                    return new ServiceResult
                    {
                        Status = Const.ERROR_VALIDATION_CODE,
                        Message = Const.ERROR_INVALID_DATA_MSG,
                    };
                }
                var battery = await _unitOfWork.BatteryRepository.GetBatteryByStationId(stationId);
                if (battery == null || !battery.Any())
                {
                    return new ServiceResult
                    {
                        Status = Const.WARNING_NO_DATA_CODE,
                        Message = Const.WARNING_NO_DATA_MSG,
                    };
                }
                return new ServiceResult
                {
                    Status = Const.SUCCESS_READ_CODE,
                    Message = Const.SUCCESS_READ_MSG,
                    Data = battery
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

        public async Task<IServiceResult> GetBatteriesByType(string typeBattery)
        {
            try
            {
                if (typeBattery.IsNullOrEmpty())
                {
                    return new ServiceResult
                    {
                        Status = Const.ERROR_VALIDATION_CODE,
                        Message = Const.ERROR_INVALID_DATA_MSG,
                    };
                }
                var battery = await _unitOfWork.BatteryRepository.GetBatteriesByType(typeBattery);
                if (battery == null || !battery.Any())
                {
                    return new ServiceResult
                    {
                        Status = Const.WARNING_NO_DATA_CODE,
                        Message = Const.WARNING_NO_DATA_MSG,
                    };
                }
                var batteryDtos = battery.Select(b => b.MapToEntity()).ToList();
                return new ServiceResult
                {
                    Status = Const.SUCCESS_READ_CODE,
                    Message = Const.SUCCESS_READ_MSG,
                    Data = battery
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

        //lấy pin theo id
        public async Task<IServiceResult> GetBatteryById(Guid batteryId)
        {
            try
            {
                if (batteryId == Guid.Empty)
                {
                    return new ServiceResult
                    {
                        Status = Const.ERROR_VALIDATION_CODE,
                        Message = Const.ERROR_INVALID_DATA_MSG,
                    };
                }
                var battery = await _unitOfWork.BatteryRepository.GetByIdAsync(batteryId);
                if (battery == null)
                {
                    return new ServiceResult
                    {
                        Status = Const.WARNING_NO_DATA_CODE,
                        Message = Const.WARNING_NO_DATA_MSG,
                    };
                }
                var BatteryDto = battery.MapToEntity();
                return new ServiceResult
                {
                    Status = Const.SUCCESS_READ_CODE,
                    Message = Const.SUCCESS_READ_MSG,
                    Data = BatteryDto
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
        //lấy số lượng pin trong trạm
        public async Task<IServiceResult> GetBatteryCountByStationId(Guid stationId)
        {
            try
            {
                if (stationId == Guid.Empty)
                {
                    return new ServiceResult
                    {
                        Status = Const.ERROR_VALIDATION_CODE,
                        Message = Const.ERROR_INVALID_DATA_MSG,
                    };
                }
                var battery = await _unitOfWork.BatteryRepository.GetBatteryCountByStationId(stationId);
                if (battery == null)
                {
                    return new ServiceResult
                    {
                        Status = Const.WARNING_NO_DATA_CODE,
                        Message = Const.WARNING_NO_DATA_MSG,
                    };
                }
                return new ServiceResult
                {
                    Status = Const.SUCCESS_READ_CODE,
                    Message = Const.SUCCESS_READ_MSG,
                    Data = battery
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult
                {
                    Status = Const.ERROR_VALIDATION_CODE,
                    Message = ex.Message,
                };
            }
        }
        // Kiểm tra pin có thể hoán đổi được không
        public async Task<IServiceResult> IsBatteryAvailable(Guid batteryId)
        {
            try
            {
                if (batteryId == Guid.Empty)
                {
                    return new ServiceResult
                    {
                        Status = Const.ERROR_VALIDATION_CODE,
                        Message = Const.ERROR_INVALID_DATA_MSG,
                    };
                }
                var battery = await _unitOfWork.BatteryRepository.GetByIdAsync(batteryId);
                if (battery == null)
                {
                    return new ServiceResult
                    {
                        Status = Const.WARNING_NO_DATA_CODE,
                        Message = Const.WARNING_NO_DATA_MSG,
                    };
                }
                var isAvailable = await _unitOfWork.BatteryRepository.IsBatteryAvailable(battery.BatteryId);
                return new ServiceResult
                {
                    Status = Const.SUCCESS_READ_CODE,
                    Message = "Battery can be swapping",
                    Data = isAvailable
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult
                {
                    Status = Const.ERROR_VALIDATION_CODE,
                    Message = ex.Message,
                };
            }
        }
        // Xoá mềm pin
        public async Task<IServiceResult> SoftDeleteBattery(Guid BatteryId)
        {
            try
            {
                if (BatteryId == Guid.Empty)
                {
                    return new ServiceResult
                    {
                        Status = Const.ERROR_VALIDATION_CODE,
                        Message = Const.ERROR_INVALID_DATA_MSG,
                    };
                }
                var battery = await _unitOfWork.BatteryRepository.GetByIdAsync(BatteryId);
                if (battery == null)
                {
                    return new ServiceResult
                    {
                        Status = Const.WARNING_NO_DATA_CODE,
                        Message = Const.WARNING_NO_DATA_MSG,
                    };
                }
                battery.Status = false;
                await _unitOfWork.BatteryRepository.UpdateAsync(battery);
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
        // Preview pin sẽ được gán khi tạo booking (dựa vào stationId và vehicleId)
        public async Task<IServiceResult> PreviewBatteryForBookingAsync(Guid stationId, Guid vehicleId)
        {
            try
            {
                // 1. Validate input
                if (stationId == Guid.Empty)
                {
                    return new ServiceResult
                    {
                        Status = 400,
                        Message = "StationId is required"
                    };
                }

                if (vehicleId == Guid.Empty)
                {
                    return new ServiceResult
                    {
                        Status = 400,
                        Message = "VehicleId is required"
                    };
                }

                // 2. Lấy thông tin xe để biết loại pin
                var car = await _unitOfWork.CarRepository.GetByIdAsync(vehicleId);
                if (car == null)
                {
                    return new ServiceResult
                    {
                        Status = 404,
                        Message = "Vehicle not found"
                    };
                }

                // 3. Lấy thông tin trạm
                var station = await _unitOfWork.StationRepository.GetByIdAsync(stationId);
                if (station == null)
                {
                    return new ServiceResult
                    {
                        Status = 404,
                        Message = "Station not found"
                    };
                }

                // 4. Tìm pin khả dụng phù hợp với loại xe tại trạm
                var battery = await _unitOfWork.BatteryRepository.GetAvailableBatteryAsync(stationId, car.BatteryType);
                
                if (battery == null)
                {
                    return new ServiceResult
                    {
                        Status = 404,
                        Message = $"No available battery of type '{car.BatteryType}' at station '{station.StationName}'",
                        Data = new
                        {
                            StationId = stationId,
                            StationName = station.StationName,
                            VehicleId = vehicleId,
                            VehicleModel = car.Model,
                            RequiredBatteryType = car.BatteryType,
                            AvailableBattery = (object)null,
                            IsAvailable = false
                        }
                    };
                }

                // 5. Trả về thông tin pin sẽ được gán
                var batteryDto = battery.MapToEntity();
                return new ServiceResult
                {
                    Status = Const.SUCCESS_READ_CODE,
                    Message = "Battery preview retrieved successfully",
                    Data = new
                    {
                        StationId = stationId,
                        StationName = station.StationName,
                        VehicleId = vehicleId,
                        VehicleModel = car.Model,
                        RequiredBatteryType = car.BatteryType,
                        AvailableBattery = batteryDto,
                        IsAvailable = true
                    }
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult
                {
                    Status = Const.ERROR_EXCEPTION,
                    Message = ex.Message
                };
            }
        }

        // Lấy danh sách pin của Station mà Staff đang làm việc
        public async Task<IServiceResult> GetBatteriesByStaffStationAsync(Guid staffAccountId)
        {
            try
            {
                // 1. Lấy Account của Staff (có Include Role và Station)
                var staff = await _unitOfWork.AccountRepository.GetAllWithRoleAndStation(staffAccountId);
                if (staff == null)
                    return new ServiceResult
                    {
                        Status = 404,
                        Message = "Staff not found"
                    };

                // 2. Validate: Phải là Staff
                if (staff.Role?.RoleName != "Staff")
                    return new ServiceResult
                    {
                        Status = 403,
                        Message = "Only Staff can access this endpoint"
                    };

                // 3. Validate: Staff phải có StationID
                if (staff.StationId == null)
                    return new ServiceResult
                    {
                        Status = 400,
                        Message = "Staff is not assigned to any station"
                    };

                // 4. Lấy danh sách Battery theo StationID
                var stationId = staff.StationId.Value;
                var batteries = await _unitOfWork.BatteryRepository.GetBatteryByStationId(stationId);

                // 5. Lấy thông tin Station để trả về kèm
                var station = await _unitOfWork.StationRepository.GetByIdAsync(stationId);

                if (batteries == null || !batteries.Any())
                    return new ServiceResult
                    {
                        Status = 404,
                        Message = "No batteries found for this station"
                    };

                // 6. Map sang DTO và trả về kèm thông tin station
                var batteryDtos = batteries.Select(b => b.MapToEntity()).ToList();
                return new ServiceResult
                {
                    Status = Const.SUCCESS_READ_CODE,
                    Message = Const.SUCCESS_READ_MSG,
                    Data = new
                    {
                        StationId = stationId,
                        StationName = station?.StationName,
                        BatteryCount = batteries.Count,
                        Batteries = batteryDtos
                    }
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult
                {
                    Status = Const.ERROR_EXCEPTION,
                    Message = ex.Message
                };
            }
        }

        // Cập nhật thông tin pin
        public async Task<IServiceResult> UpdateBatteryAsync(UpdateBattery updateDTO)
        {
            try
            {
                if (updateDTO == null)
                {
                    return new ServiceResult
                    {
                        Status = Const.ERROR_VALIDATION_CODE,
                        Message = Const.ERROR_INVALID_DATA_MSG,
                    };
                }
                var battery = await _unitOfWork.BatteryRepository.GetByIdAsync(updateDTO.BatteryId);
                if (battery == null)
                {
                    return new ServiceResult
                    {
                        Status = Const.WARNING_NO_DATA_CODE,
                        Message = Const.WARNING_NO_DATA_MSG,
                    };
                }
                battery.MaptoUpdate(updateDTO);
                await _unitOfWork.BatteryRepository.UpdateAsync(battery);
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
    }
}
