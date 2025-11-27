using EV_BatteryChangeStation_Common.DTOs.SwappingtransactionDto;
using EV_BatteryChangeStation_Common.Enum.ServiceResult;
using EV_BatteryChangeStation_Common.Enum.SwappingTransactionEnum;
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
    public class SwappingService : ISwappingService
    {
        private readonly IUnitOfWork _unitOfWork;
        public SwappingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentException(nameof(unitOfWork));
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

                // 1. Kiểm tra pin có tồn tại và khả dụng không
                var battery = await _unitOfWork.BatteryRepository.GetByIdAsync(createSwappingDto.NewBatteryId);
                if (battery == null)
                {
                    return new ServiceResult
                    {
                        Status = 404,
                        Message = "Battery not found"
                    };
                }

                if (battery.Status != true)
                {
                    return new ServiceResult
                    {
                        Status = 400,
                        Message = "Battery is not available for swapping"
                    };
                }

                // 2. Lấy thông tin Station để trừ số lượng pin
                var station = await _unitOfWork.StationRepository.GetByIdAsync(battery.StationId);
                if (station == null)
                {
                    return new ServiceResult
                    {
                        Status = 404,
                        Message = "Station not found"
                    };
                }

                // 3. Tạo SwappingTransaction
                var swapping = createSwappingDto.MaptoCreate();
                await _unitOfWork.SwappingTransactionRepository.CreateAsync(swapping);

                // 4. Đánh dấu pin đã được sử dụng (Status = false) → không hiển thị trong danh sách
                battery.Status = false;
                battery.LastUsed = DateTime.Now;
                battery.BatterySwapDate = DateTime.Now;
                await _unitOfWork.BatteryRepository.UpdateAsync(battery);

                // 5. Trừ số lượng pin trong Station
                if (station.BatteryQuantity.HasValue && station.BatteryQuantity > 0)
                {
                    station.BatteryQuantity -= 1;
                    _unitOfWork.StationRepository.Update(station);
                }

                // 6. Commit tất cả thay đổi
                await _unitOfWork.CommitAsync();

                return new ServiceResult
                {
                    Status = Const.SUCCESS_CREATE_CODE,
                    Message = "Swapping transaction created successfully. Battery has been removed from station inventory.",
                    Data = new
                    {
                        TransactionId = swapping.TransactionId,
                        BatteryId = battery.BatteryId,
                        StationId = station.StationId,
                        StationName = station.StationName,
                        RemainingBatteries = station.BatteryQuantity
                    }
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
        public async Task<IServiceResult> DeleteTransactionAsync(Guid transactionId)
        {
            try
            {
                if (transactionId == Guid.Empty)
                {
                    return new ServiceResult
                    {
                        Status = Const.ERROR_VALIDATION_CODE,
                        Message = Const.ERROR_INVALID_DATA_MSG,
                    };
                }
                var swaping = await _unitOfWork.SwappingTransactionRepository.GetByIdAsync(transactionId);
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
        public async Task<IServiceResult> GetTransactionByCarIdAsync(Guid carid)
        {
            try
            {
                if(carid == Guid.Empty)
                {
                    return new ServiceResult
                    {
                        Status = Const.ERROR_VALIDATION_CODE,
                        Message = Const.ERROR_INVALID_DATA_MSG,
                    };
                }
                var swapping = await _unitOfWork.SwappingTransactionRepository.getByCarId(carid);
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
        public async Task<IServiceResult> GetTransactionByIdAsync(Guid transactionId)
        {
            try
            {
                if (transactionId == Guid.Empty)
                {
                    return new ServiceResult
                    {
                        Status = Const.ERROR_VALIDATION_CODE,
                        Message = Const.ERROR_INVALID_DATA_MSG,
                    };
                }
                var swapping = await _unitOfWork.SwappingTransactionRepository.GetByIdAsync(transactionId);
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
        public async Task<IServiceResult> SoftDeleteTransactionAsync(Guid transactionid)
        {
            try
            {
                if (transactionid == Guid.Empty)
                {
                    return new ServiceResult
                    {
                        Status = Const.ERROR_VALIDATION_CODE,
                        Message = Const.ERROR_INVALID_DATA_MSG,
                    };
                }
                var swap = await _unitOfWork.SwappingTransactionRepository.GetByIdAsync(transactionid);
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
                var swap = await _unitOfWork.SwappingTransactionRepository.GetByIdAsync(updateSwappingDto.TransactionId);
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

        /// <summary>
        /// Staff xác nhận đổi pin sau khi kiểm tra payment đã thành công
        /// Flow: Booking (Approved) + Payment (Successful) → SwappingTransaction + Trừ pin khỏi kho
        /// </summary>
        public async Task<IServiceResult> ConfirmAndSwapAsync(ConfirmSwapDTO dto)
        {
            try
            {
                // 1. Validate input
                if (dto == null || dto.BookingId == Guid.Empty)
                {
                    return new ServiceResult
                    {
                        Status = 400,
                        Message = "BookingId is required"
                    };
                }

                if (dto.StaffId == Guid.Empty)
                {
                    return new ServiceResult
                    {
                        Status = 400,
                        Message = "StaffId is required"
                    };
                }

                // 2. Kiểm tra Staff có tồn tại và có quyền không
                var staff = await _unitOfWork.AccountRepository.GetAllWithRoleAndStation(dto.StaffId);
                if (staff == null)
                {
                    return new ServiceResult
                    {
                        Status = 404,
                        Message = "Staff not found"
                    };
                }

                if (staff.Role?.RoleName != "Staff")
                {
                    return new ServiceResult
                    {
                        Status = 403,
                        Message = "Only Staff can confirm swapping"
                    };
                }

                // 3. Lấy thông tin Booking
                var booking = await _unitOfWork.BookingRepository.GetByIdAsync(dto.BookingId);
                if (booking == null)
                {
                    return new ServiceResult
                    {
                        Status = 404,
                        Message = "Booking not found"
                    };
                }

                // 4. Kiểm tra Staff có thuộc Station của Booking không
                if (staff.StationId != booking.StationId)
                {
                    return new ServiceResult
                    {
                        Status = 403,
                        Message = "Staff can only process bookings at their assigned station"
                    };
                }

                // 5. Kiểm tra trạng thái Booking phải là Approved (đã được staff xác nhận)
                if (booking.IsApproved != "Approved")
                {
                    return new ServiceResult
                    {
                        Status = 400,
                        Message = $"Cannot process booking with status '{booking.IsApproved}'. Booking must be 'Approved' before swapping."
                    };
                }

                // 6. Kiểm tra Booking có BatteryId không
                if (!booking.BatteryId.HasValue)
                {
                    return new ServiceResult
                    {
                        Status = 400,
                        Message = "Booking does not have a battery assigned"
                    };
                }

                // 7. Lấy thông tin Battery
                var battery = await _unitOfWork.BatteryRepository.GetByIdAsync(booking.BatteryId.Value);
                if (battery == null)
                {
                    return new ServiceResult
                    {
                        Status = 404,
                        Message = "Battery not found"
                    };
                }

                if (battery.Status != true)
                {
                    return new ServiceResult
                    {
                        Status = 400,
                        Message = "Battery is no longer available"
                    };
                }

                // 8. Kiểm tra Subscription (gói) của user còn hiệu lực không
                // Logic: Kiểm tra qua Payment thay vì Subscription trực tiếp
                var activePayment = await _unitOfWork.PaymentRepository.GetActiveSubscriptionPaymentByAccountIdAsync(booking.AccountId);
                if (activePayment == null || activePayment.Subscription == null)
                {
                    return new ServiceResult
                    {
                        Status = 400,
                        Message = "User does not have an active subscription or no remaining swaps"
                    };
                }

                var subscription = activePayment.Subscription;

                // Nếu gói có giới hạn lượt thì trừ 1 lượt
                if (subscription.RemainingSwaps.HasValue)
                {
                    if (subscription.RemainingSwaps <= 0)
                    {
                        return new ServiceResult
                        {
                            Status = 400,
                            Message = "No remaining swaps in current subscription"
                        };
                    }

                    subscription.RemainingSwaps -= 1;

                    // Nếu cần, có thể tự động inactive khi hết lượt
                    if (subscription.RemainingSwaps == 0)
                    {
                        subscription.IsActive = false;
                    }

                    _unitOfWork.SubscriptionRepository.Update(subscription);
                }

                // 9. Lấy thông tin Station
                var station = await _unitOfWork.StationRepository.GetByIdAsync(booking.StationId);
                if (station == null)
                {
                    return new ServiceResult
                    {
                        Status = 404,
                        Message = "Station not found"
                    };
                }

                // 10. Tạo SwappingTransaction
                var swappingTransaction = new SwappingTransaction
                {
                    TransactionId = Guid.NewGuid(),
                    StaffId = dto.StaffId,
                    VehicleId = booking.VehicleId,
                    NewBatteryId = booking.BatteryId.Value,
                    Notes = dto.Notes ?? $"Swap from Booking #{booking.BookingId}",
                    Status = SwappingEnum.Active.ToString(),
                    CreateDate = DateTime.Now
                };

                await _unitOfWork.SwappingTransactionRepository.CreateAsync(swappingTransaction);

                // 11. Đánh dấu pin đã được sử dụng (Status = false) → không hiển thị trong danh sách
                battery.Status = false;
                battery.LastUsed = DateTime.Now;
                battery.BatterySwapDate = DateTime.Now;
                await _unitOfWork.BatteryRepository.UpdateAsync(battery);

                // 12. Trừ số lượng pin trong Station
                if (station.BatteryQuantity.HasValue && station.BatteryQuantity > 0)
                {
                    station.BatteryQuantity -= 1;
                    _unitOfWork.StationRepository.Update(station);
                }

                // 13. Cập nhật trạng thái Booking thành Completed
                booking.IsApproved = "Completed";
                _unitOfWork.BookingRepository.Update(booking);

                // 14. Commit tất cả thay đổi
                await _unitOfWork.CommitAsync();

                return new ServiceResult
                {
                    Status = 200,
                    Message = "Swapping completed successfully. Battery has been removed from station inventory.",
                    Data = new
                    {
                        TransactionId = swappingTransaction.TransactionId,
                        BookingId = booking.BookingId,
                        BatteryId = battery.BatteryId,
                        VehicleId = booking.VehicleId,
                        StationId = station.StationId,
                        StationName = station.StationName,
                        StaffId = dto.StaffId,
                        StaffName = staff.FullName,
                        RemainingBatteries = station.BatteryQuantity,
                        CompletedAt = DateTime.Now
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
    }
}
