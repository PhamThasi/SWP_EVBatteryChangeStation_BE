using EV_BatteryChangeStation_Common.DTOs.SwappingtransactionDto;
using EV_BatteryChangeStation_Common.Enum.ServiceResult;
using EV_BatteryChangeStation_Common.Enum.SwappingTransactionEnum;
using EV_BatteryChangeStation_Repository.Entities;
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
    public class SwappingService : ISwappingService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly Hashids _hashids;
        public SwappingService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentException(nameof(unitOfWork));
            _hashids = new Hashids("EV_BatteryChangeStation", 10);
        }
        // Create Swapping Transaction
        public async Task<IServiceResult> CreateTransactionAsync(CreateSwappingDto createSwappingDto)
        {
            try
            {
                if (createSwappingDto == null)
                {
                    return new ServiceResult
                    {
                        Status = Const.ERROR_VALIDATION_CODE,
                        Message = Const.ERROR_INVALID_DATA_MSG,
                    };
                }
                var swapping = createSwappingDto.MaptoCreate();
                await _unitOfWork.SwappingTransactionRepository.CreateAsync(swapping);
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
        // Delete Swapping Transaction
        public async Task<IServiceResult> DeleteTransactionAsync(string transactionId)
        {
            try
            {
                if (transactionId == null)
                {
                    return new ServiceResult
                    {
                        Status = Const.ERROR_VALIDATION_CODE,
                        Message = Const.ERROR_INVALID_DATA_MSG,
                    };
                }
                var swaping = await _unitOfWork.SwappingTransactionRepository.GetByIdAsync(_hashids.DecodeSingle(transactionId));
                if (swaping == null)
                {
                    return new ServiceResult
                    {
                        Status = Const.FAIL_READ_CODE,
                        Message = Const.FAIL_READ_MSG,
                    };
                }
                await _unitOfWork.SwappingTransactionRepository.RemoveAsync(swaping);
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
        // Get All Swapping Transactions
        public async Task<IServiceResult> GetAllTransactionsAsync()
        {
            try
            {
                var swapping = await _unitOfWork.SwappingTransactionRepository.GetAllSwappingTransactionDetail();
                if (swapping == null || !swapping.Any())
                {
                    return new ServiceResult
                    {
                        Status = Const.WARNING_NO_DATA_CODE,
                        Message = Const.WARNING_NO_DATA_MSG,
                    };
                }
                var swpdto = swapping.Select(b => b.MaptoEntity()).ToList();
                return new ServiceResult
                {
                    Status = Const.SUCCESS_READ_CODE,
                    Message = Const.SUCCESS_READ_MSG,
                    Data = swpdto
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
        // Get Swapping Transaction by Car Id
        public async Task<IServiceResult> GetTransactionByCarIdAsync(string carid)
        {
            try
            {
                if(carid.IsNullOrEmpty())
                {
                    return new ServiceResult
                    {
                        Status = Const.ERROR_VALIDATION_CODE,
                        Message = Const.ERROR_INVALID_DATA_MSG,
                    };
                }
                var swapping = await _unitOfWork.SwappingTransactionRepository.getByCarId(_hashids.DecodeSingle(carid));
                if (swapping == null || !swapping.Any())
                {
                    return new ServiceResult
                    {
                        Status = Const.WARNING_NO_DATA_CODE,
                        Message = Const.WARNING_NO_DATA_MSG,
                    };
                }
                var swpdto = swapping.Select(b => b.MaptoEntity()).ToList();
                return new ServiceResult
                {
                    Status = Const.SUCCESS_READ_CODE,
                    Message = Const.SUCCESS_READ_MSG,
                    Data = swpdto
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
        // Get Swapping Transaction by Id
        public async Task<IServiceResult> GetTransactionByIdAsync(string transactionId)
        {
            try
            {
                if (transactionId == null)
                {
                    return new ServiceResult
                    {
                        Status = Const.ERROR_VALIDATION_CODE,
                        Message = Const.ERROR_INVALID_DATA_MSG,
                    };
                }
                var swapping = await _unitOfWork.SwappingTransactionRepository.GetByIdAsync(_hashids.DecodeSingle(transactionId));
                if (swapping == null)
                {
                    return new ServiceResult
                    {
                        Status = Const.WARNING_NO_DATA_CODE,
                        Message = Const.WARNING_NO_DATA_MSG,
                    };
                }
                var swapdto = swapping.MaptoEntity();
                return new ServiceResult
                {
                    Status = Const.SUCCESS_READ_CODE,
                    Message = Const.SUCCESS_READ_MSG,
                    Data = swapdto
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
        // Soft Delete Swapping Transaction
        public async Task<IServiceResult> SoftDeleteTransactionAsync(string transactionid)
        {
            try
            {
                if (transactionid == null)
                {
                    return new ServiceResult
                    {
                        Status = Const.ERROR_VALIDATION_CODE,
                        Message = Const.ERROR_INVALID_DATA_MSG,
                    };
                }
                var swap = await _unitOfWork.SwappingTransactionRepository.GetByIdAsync(_hashids.DecodeSingle(transactionid));
                if (swap == null)
                {
                    return new ServiceResult
                    {
                        Status = Const.WARNING_NO_DATA_CODE,
                        Message = Const.WARNING_NO_DATA_MSG,
                    };
                }
                swap.Status =  SwappingEnum.Pending.ToString();
                await _unitOfWork.SwappingTransactionRepository.UpdateAsync(swap);
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
        // Update Swapping Transaction
        public async Task<IServiceResult> UpdateTransactionAsync(UpdateSwappingDto updateSwappingDto)
        {
            try
            {
                if (updateSwappingDto == null)
                {
                    return new ServiceResult
                    {
                        Status = Const.ERROR_VALIDATION_CODE,
                        Message = Const.ERROR_INVALID_DATA_MSG,
                    };
                }
                var swap = await _unitOfWork.SwappingTransactionRepository.GetByIdAsync(_hashids.DecodeSingle(updateSwappingDto.TransactionId));
                if (swap == null)
                {
                    return new ServiceResult
                    {
                        Status = Const.WARNING_NO_DATA_CODE,
                        Message = Const.WARNING_NO_DATA_MSG,
                    };
                }
                swap.MaptoUpdate(updateSwappingDto);
                await _unitOfWork.SwappingTransactionRepository.UpdateAsync(swap);
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
