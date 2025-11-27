using EV_BatteryChangeStation_Common.DTOs.BookingDTO;
using EV_BatteryChangeStation_Common.Enum.BookingEnum;
using EV_BatteryChangeStation_Repository.Mapper;
using EV_BatteryChangeStation_Repository.UnitOfWork;
using EV_BatteryChangeStation_Service.Base;
using EV_BatteryChangeStation_Service.InternalService.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Service.InternalService.Service
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Lấy tất cả booking (bao gồm cả đã hủy)
        public async Task<ServiceResult> GetAllAsync()
        {
            try
            {
                var bookings = await _unitOfWork.BookingRepository.GetAllAsync();
                var result = bookings.Select(BookingMapper.ToDTO).ToList();

                return new ServiceResult(200, "Success", result, BookingErrorCode.None);
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Error fetching bookings", new List<string> { ex.Message }, BookingErrorCode.DatabaseError);
            }
        }

        // Lấy booking theo ID
        public async Task<ServiceResult> GetByIdAsync(Guid id)
        {
            try
            {
                var booking = await _unitOfWork.BookingRepository.GetByIdAsync(id);
                if (booking == null)
                    return new ServiceResult(404, "Booking not found", null, BookingErrorCode.BookingNotFound);

                return new ServiceResult(200, "Success", BookingMapper.ToDTO(booking), BookingErrorCode.None);
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Error fetching booking", new List<string> { ex.Message }, BookingErrorCode.UnexpectedError);
            }
        }

        // Tạo booking mới
        public async Task<ServiceResult> CreateAsync(BookingCreateDTO dto)
        {
            try
            {
                if (dto == null)
                    return new ServiceResult(400, "Booking data is missing", null, BookingErrorCode.MissingRequiredField);

                if (dto.DateTime < DateTime.Now)
                    return new ServiceResult(400, "Booking time cannot be in the past", null, BookingErrorCode.TimeInPast);

                // Kiểm tra user có payment thành công với subscription active không
                // Logic: Kiểm tra qua Payment thay vì Subscription trực tiếp
                var activePayment = await _unitOfWork.PaymentRepository.GetActiveSubscriptionPaymentByAccountIdAsync(dto.AccountId);
                if (activePayment == null || activePayment.Subscription == null)
                {
                    return new ServiceResult(400, "You must have an active subscription to create a booking. Please purchase a subscription first.", null, BookingErrorCode.MissingRequiredField);
                }

                var subscription = activePayment.Subscription;

                // Kiểm tra subscription còn hiệu lực (đã được check trong GetActiveSubscriptionPaymentByAccountIdAsync nhưng double check)
                if (subscription.EndDate.HasValue && subscription.EndDate < DateTime.Now)
                {
                    return new ServiceResult(400, "Your subscription has expired. Please renew your subscription.", null, BookingErrorCode.MissingRequiredField);
                }

                // Kiểm tra còn lượt swap không (nếu có giới hạn)
                if (subscription.RemainingSwaps.HasValue && subscription.RemainingSwaps <= 0)
                {
                    return new ServiceResult(400, "You have no remaining swaps in your subscription. Please renew your subscription.", null, BookingErrorCode.MissingRequiredField);
                }

                var existing = (await _unitOfWork.BookingRepository.GetAllAsync())
                    .FirstOrDefault(b => b.StationId == dto.StationId &&
                                         b.DateTime == dto.DateTime);

                if (existing != null)
                    return new ServiceResult(409, "Duplicate booking for this time slot", null, BookingErrorCode.DuplicateBooking);

                var batteryAssignment = await TryAssignBatteryAsync(dto.StationId, dto.VehicleId);
                if (!batteryAssignment.IsSuccess)
                {
                    return batteryAssignment.ErrorResult;
                }
                dto.BatteryId = batteryAssignment.BatteryId;

                var entity = BookingMapper.ToEntity(dto);
                entity.CreatedDate = DateTime.Now;
                entity.IsApproved = Convert.ToString(BookingApprovalStatus.Pending);

                await _unitOfWork.BookingRepository.AddAsync(entity);
                await _unitOfWork.CommitAsync();

                return new ServiceResult(201, "Booking created successfully", BookingMapper.ToDTO(entity), BookingErrorCode.None);
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Error creating booking", new List<string> { ex.Message }, BookingErrorCode.TransactionFailed);
            }
        }

        // Cập nhật booking (nếu chưa bị hủy)
        public async Task<ServiceResult> UpdateAsync(Guid id, BookingCreateDTO dto)
        {
            try
            {
                var existing = await _unitOfWork.BookingRepository.GetByIdAsync(id);
                if (existing == null)
                    return new ServiceResult(404, "Booking not found", null, BookingErrorCode.BookingNotFound);

                if (existing.IsApproved == Convert.ToString(BookingApprovalStatus.Canceled))
                    return new ServiceResult(400, "Cannot update a cancelled booking", null, BookingErrorCode.BookingAlreadyCancelled);

                var batteryAssignment = await TryAssignBatteryAsync(dto.StationId, dto.VehicleId);
                if (!batteryAssignment.IsSuccess)
                {
                    return batteryAssignment.ErrorResult;
                }
                dto.BatteryId = batteryAssignment.BatteryId;

                BookingMapper.UpdateEntity(existing, dto);
                _unitOfWork.BookingRepository.Update(existing);
                await _unitOfWork.CommitAsync();

                return new ServiceResult(200, "Booking updated successfully", BookingMapper.ToDTO(existing), BookingErrorCode.None);
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Error updating booking", new List<string> { ex.Message }, BookingErrorCode.DatabaseError);
            }
        }

        // Soft delete — chuyển trạng thái thành Canceled
        public async Task<ServiceResult> DeleteAsync(Guid id)
        {
            try
            {
                var existing = await _unitOfWork.BookingRepository.GetByIdAsync(id);
                if (existing == null)
                    return new ServiceResult(404, "Booking not found", null, BookingErrorCode.BookingNotFound);

                if (existing.IsApproved == Convert.ToString(BookingApprovalStatus.Canceled))
                    return new ServiceResult(400, "Booking is already cancelled", null, BookingErrorCode.BookingAlreadyCancelled);

                existing.IsApproved = Convert.ToString(BookingApprovalStatus.Canceled);
                _unitOfWork.BookingRepository.Update(existing);
                await _unitOfWork.CommitAsync();

                return new ServiceResult(200, "Booking cancelled successfully", null, BookingErrorCode.None);
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Error cancelling booking", new List<string> { ex.Message }, BookingErrorCode.TransactionFailed);
            }
        }

        // Hard delete — xóa khỏi DB
        public async Task<ServiceResult> HardDeleteAsync(Guid id)
        {
            try
            {
                var existing = await _unitOfWork.BookingRepository.GetByIdAsync(id);
                if (existing == null)
                    return new ServiceResult(404, "Booking not found for hard delete", null, BookingErrorCode.BookingNotFound);

                _unitOfWork.BookingRepository.Delete(existing);
                await _unitOfWork.CommitAsync();

                return new ServiceResult(200, "Booking permanently deleted", null, BookingErrorCode.None);
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Error permanently deleting booking", new List<string> { ex.Message }, BookingErrorCode.DatabaseError);
            }
        }

        // Lấy tất cả booking theo AccountId
        public async Task<ServiceResult> GetByAccountIdAsync(Guid accountId)
        {
            try
            {
                var bookings = await _unitOfWork.BookingRepository.GetByAccountIdAsync(accountId);
                if (bookings == null || !bookings.Any())
                    return new ServiceResult(404, "No bookings found for this user", null, BookingErrorCode.BookingNotFound);

                var result = bookings.Select(BookingMapper.ToDTO).ToList();
                return new ServiceResult(200, "Success", result, BookingErrorCode.None);
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Error fetching user bookings", new List<string> { ex.Message }, BookingErrorCode.DatabaseError);
            }
        }

        // Hiển<Task>: Lấy booking theo Station của Staff - Validate Role và StationID
        // Lấy booking theo Station của Staff
        public async Task<ServiceResult> GetByStaffStationAsync(Guid staffAccountId)
        {
            try
            {
                // 1. Lấy Account của Staff (có Include Role và Station)
                var staff = await _unitOfWork.AccountRepository.GetAllWithRoleAndStation(staffAccountId);
                if (staff == null)
                    return new ServiceResult(404, "Staff not found", null, BookingErrorCode.BookingNotFound);

                // 2. Validate: Phải là Staff
                if (staff.Role?.RoleName != "Staff")
                    return new ServiceResult(403, "Only Staff can access this endpoint", null, BookingErrorCode.BookingNotFound);

                // 3. Validate: Staff phải có StationID
                if (staff.StationId == null)
                    return new ServiceResult(400, "Staff is not assigned to any station", null, BookingErrorCode.BookingNotFound);

                // 4. Lấy Booking theo StationID
                var stationId = staff.StationId.Value;
                var bookings = await _unitOfWork.BookingRepository.GetByStationIdAsync(stationId);
                
                if (bookings == null || !bookings.Any())
                    return new ServiceResult(404, "No bookings found for this station", null, BookingErrorCode.BookingNotFound);

                // 5. Map sang DTO và trả về
                var result = bookings.Select(BookingMapper.ToDTO).ToList();
                return new ServiceResult(200, "Success", result, BookingErrorCode.None);
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Error fetching bookings", new List<string> { ex.Message }, BookingErrorCode.DatabaseError);
            }
        }

        /// <summary>
        /// Staff xác nhận hoặc từ chối booking (chuyển trạng thái Pending → Approved/Rejected)
        /// </summary>
        public async Task<ServiceResult> UpdateBookingStatusAsync(Guid bookingId, string status, Guid staffId, string? notes = null)
        {
            try
            {
                // 1. Validate status phải là Approved hoặc Rejected
                if (status != "Approved" && status != "Rejected")
                {
                    return new ServiceResult(400, "Status must be 'Approved' or 'Rejected'", null, BookingErrorCode.MissingRequiredField);
                }

                // 2. Kiểm tra Staff có tồn tại và có quyền không
                var staff = await _unitOfWork.AccountRepository.GetAllWithRoleAndStation(staffId);
                if (staff == null)
                {
                    return new ServiceResult(404, "Staff not found", null, BookingErrorCode.BookingNotFound);
                }

                if (staff.Role?.RoleName != "Staff")
                {
                    return new ServiceResult(403, "Only Staff can approve/reject bookings", null, BookingErrorCode.BookingNotFound);
                }

                // 3. Lấy thông tin Booking
                var booking = await _unitOfWork.BookingRepository.GetByIdAsync(bookingId);
                if (booking == null)
                {
                    return new ServiceResult(404, "Booking not found", null, BookingErrorCode.BookingNotFound);
                }

                // 4. Kiểm tra Staff có thuộc Station của Booking không
                if (staff.StationId != booking.StationId)
                {
                    return new ServiceResult(403, "Staff can only process bookings at their assigned station", null, BookingErrorCode.BookingNotFound);
                }

                // 5. Kiểm tra trạng thái Booking phải là Pending
                if (booking.IsApproved != "Pending")
                {
                    return new ServiceResult(400, $"Cannot update booking with status '{booking.IsApproved}'. Only 'Pending' bookings can be approved/rejected.", null, BookingErrorCode.BookingAlreadyCancelled);
                }

                // 6. Cập nhật trạng thái Booking
                booking.IsApproved = status;
                if (!string.IsNullOrEmpty(notes))
                {
                    booking.Notes = string.IsNullOrEmpty(booking.Notes) 
                        ? notes 
                        : $"{booking.Notes}\n[Staff Note]: {notes}";
                }

                _unitOfWork.BookingRepository.Update(booking);
                await _unitOfWork.CommitAsync();

                var message = status == "Approved" 
                    ? "Booking approved successfully" 
                    : "Booking rejected successfully";

                return new ServiceResult(200, message, BookingMapper.ToDTO(booking), BookingErrorCode.None);
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Error updating booking status", new List<string> { ex.Message }, BookingErrorCode.DatabaseError);
            }
        }

        private async Task<(bool IsSuccess, Guid BatteryId, ServiceResult ErrorResult)> TryAssignBatteryAsync(Guid stationId, Guid vehicleId)
        {
            var car = await _unitOfWork.CarRepository.GetByIdAsync(vehicleId);
            if (car == null)
            {
                return (false,
                        Guid.Empty,
                        new ServiceResult(404, "Vehicle not found", null, BookingErrorCode.VehicleNotFound));
            }

            var battery = await _unitOfWork.BatteryRepository.GetAvailableBatteryAsync(stationId, car.BatteryType);
            if (battery == null)
            {
                return (false,
                        Guid.Empty,
                        new ServiceResult(404, "No available battery matches this vehicle at the station", null, BookingErrorCode.StationNotAvailable));
            }

            return (true, battery.BatteryId, null);
        }
    }
}
