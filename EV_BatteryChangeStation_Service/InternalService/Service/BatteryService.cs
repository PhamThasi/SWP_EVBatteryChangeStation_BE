using EV_BatteryChangeStation_Common.DTOs.BatteryDTO;
using EV_BatteryChangeStation_Common.Enum.ServiceResult;
using EV_BatteryChangeStation_Repository.Mapper;
using EV_BatteryChangeStation_Repository.UnitOfWork;
using EV_BatteryChangeStation_Service.Base;
using EV_BatteryChangeStation_Service.InternalService.IService;
using HashidsNet;
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
        private readonly UnitOfWork _unitOfWork;
        private readonly Hashids _hashids;
        public BatteryService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentException(nameof(unitOfWork));
            _hashids = new Hashids("EV_BatteryChangeStation", 10);
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
        public async Task<IServiceResult> DeleteBattery(string batteryId)
        {
            try
            {
                if (batteryId.IsNullOrEmpty())
                {
                    return new ServiceResult
                    {
                        Status = Const.ERROR_VALIDATION_CODE,
                        Message = Const.ERROR_INVALID_DATA_MSG,
                    };
                }
                var battery = await _unitOfWork.BatteryRepository.GetByIdAsync(_hashids.DecodeSingle(batteryId));
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
        public async Task<IServiceResult> GetAllBatteryByStationId(int stationId)
        {
            try
            {
                if (stationId == 0)
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
        //lấy pin theo id
        public async Task<IServiceResult> GetBatteryById(string batteryId)
        {
            try
            {
                if (batteryId == null)
                {
                    return new ServiceResult
                    {
                        Status = Const.ERROR_VALIDATION_CODE,
                        Message = Const.ERROR_INVALID_DATA_MSG,
                    };
                }
                var battery = await _unitOfWork.BatteryRepository.GetByIdAsync(_hashids.DecodeSingle(batteryId));
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
        public async Task<IServiceResult> GetBatteryCountByStationId(int stationId)
        {
            try
            {
                if (stationId == 0)
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
        public async Task<IServiceResult> IsBatteryAvailable(string batteryId)
        {
            try
            {
                if (batteryId.IsNullOrEmpty())
                {
                    return new ServiceResult
                    {
                        Status = Const.ERROR_VALIDATION_CODE,
                        Message = Const.ERROR_INVALID_DATA_MSG,
                    };
                }
                var battery = await _unitOfWork.BatteryRepository.GetByIdAsync(_hashids.DecodeSingle(batteryId));
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
        public async Task<IServiceResult> SoftDeleteBaterry(string BatteryId)
        {
            try
            {
                if (BatteryId == null)
                {
                    return new ServiceResult
                    {
                        Status = Const.ERROR_VALIDATION_CODE,
                        Message = Const.ERROR_INVALID_DATA_MSG,
                    };
                }
                var battery = await _unitOfWork.BatteryRepository.GetByIdAsync(_hashids.DecodeSingle(BatteryId));
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
                var battery = await _unitOfWork.BatteryRepository.GetByIdAsync(_hashids.DecodeSingle(updateDTO.BatteryId));
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
