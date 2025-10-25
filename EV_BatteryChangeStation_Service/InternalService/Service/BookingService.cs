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

        // Lấy tất cả booking còn hoạt động
        public async Task<ServiceResult> GetAllAsync()
        {
            try
            {
                var bookings = await _unitOfWork.BookingRepository.GetAllAsync();
                var activeBookings = bookings
                    .Where(b => b.Status == true)
                    .Select(BookingMapper.ToDTO)
                    .ToList();

                return new ServiceResult(200, "Success", activeBookings, BookingErrorCode.None);
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

                if (booking == null || booking.Status == false)
                    return new ServiceResult(404, "Booking not found or cancelled", null, BookingErrorCode.BookingNotFound);

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

                var existing = (await _unitOfWork.BookingRepository.GetAllAsync())
                    .FirstOrDefault(b => b.StationId == dto.StationId && b.DateTime == dto.DateTime && (b.Status ?? false));

                if (existing != null)
                    return new ServiceResult(409, "Duplicate booking for this time slot", null, BookingErrorCode.DuplicateBooking);

                var entity = BookingMapper.ToEntity(dto);
                entity.CreatedDate = DateTime.Now;
                entity.Status = true;

                await _unitOfWork.BookingRepository.AddAsync(entity);
                await _unitOfWork.CommitAsync();

                return new ServiceResult(201, "Booking created successfully", BookingMapper.ToDTO(entity), BookingErrorCode.None);
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Error creating booking", new List<string> { ex.Message }, BookingErrorCode.TransactionFailed);
            }
        }

        // Cập nhật booking (nếu chưa hủy)
        public async Task<ServiceResult> UpdateAsync(Guid id, BookingCreateDTO dto)
        {
            try
            {
                var existing = await _unitOfWork.BookingRepository.GetByIdAsync(id);
                if (existing == null)
                    return new ServiceResult(404, "Booking not found", null, BookingErrorCode.BookingNotFound);

                if (existing.Status == false)
                    return new ServiceResult(400, "Cannot update a cancelled booking", null, BookingErrorCode.BookingAlreadyCancelled);

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

        // Xóa mềm (đánh dấu đã hủy)
        public async Task<ServiceResult> DeleteAsync(Guid id)
        {
            try
            {
                var existing = await _unitOfWork.BookingRepository.GetByIdAsync(id);
                if (existing == null)
                    return new ServiceResult(404, "Booking not found", null, BookingErrorCode.BookingNotFound);

                if (existing.Status == false)
                    return new ServiceResult(400, "Booking is already cancelled", null, BookingErrorCode.BookingAlreadyCancelled);

                existing.Status = false;
                _unitOfWork.BookingRepository.Update(existing);
                await _unitOfWork.CommitAsync();

                return new ServiceResult(200, "Booking cancelled successfully", null, BookingErrorCode.None);
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Error cancelling booking", new List<string> { ex.Message }, BookingErrorCode.TransactionFailed);
            }
        }

        // Xóa cứng (xóa khỏi DB)
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
    }
}
