using EV_BatteryChangeStation_Common.DTOs.BookingDTO;
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

        // 🟢 Lấy tất cả booking còn hoạt động (Status == true)
        public async Task<ServiceResult> GetAllAsync()
        {
            try
            {
                var bookings = await _unitOfWork.BookingRepository.GetAllAsync();

                // Lọc ra chỉ các booking đang hoạt động
                var activeBookings = bookings
                    .Where(b => b.Status == true)
                    .Select(BookingMapper.ToDTO)
                    .ToList();

                return new ServiceResult(200, "Success", activeBookings);
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Error fetching bookings", new List<string> { ex.Message });
            }
        }

        // 🟢 Lấy booking theo ID, nhưng không hiển thị nếu đã bị hủy
        public async Task<ServiceResult> GetByIdAsync(int id)
        {
            try
            {
                var booking = await _unitOfWork.BookingRepository.GetByIdAsync(id);

                if (booking == null || booking.Status == false)
                    return new ServiceResult(404, "Booking not found or has been cancelled");

                return new ServiceResult(200, "Success", BookingMapper.ToDTO(booking));
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Error fetching booking", new List<string> { ex.Message });
            }
        }

        // 🟢 Tạo booking mới
        public async Task<ServiceResult> CreateAsync(BookingDTO dto)
        {
            try
            {
                var entity = BookingMapper.ToEntity(dto);
                entity.CreatedDate = DateTime.Now;
                entity.Status = true; // Mặc định là còn hoạt động

                await _unitOfWork.BookingRepository.AddAsync(entity);
                await _unitOfWork.CommitAsync();

                return new ServiceResult(201, "Booking created successfully", BookingMapper.ToDTO(entity));
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Error creating booking", new List<string> { ex.Message });
            }
        }

        // 🟢 Cập nhật thông tin booking (nếu chưa bị hủy)
        public async Task<ServiceResult> UpdateAsync(int id, BookingDTO dto)
        {
            try
            {
                var existing = await _unitOfWork.BookingRepository.GetByIdAsync(id);
                if (existing == null || existing.Status == false)
                    return new ServiceResult(404, "Booking not found or has been cancelled");

                BookingMapper.UpdateEntity(existing, dto);
                _unitOfWork.BookingRepository.Update(existing);
                await _unitOfWork.CommitAsync();

                return new ServiceResult(200, "Booking updated successfully", BookingMapper.ToDTO(existing));
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Error updating booking", new List<string> { ex.Message });
            }
        }

        // 🟢 Xóa mềm (chỉ cập nhật trạng thái)
        public async Task<ServiceResult> DeleteAsync(int id)
        {
            try
            {
                var existing = await _unitOfWork.BookingRepository.GetByIdAsync(id);
                if (existing == null)
                    return new ServiceResult(404, "Booking not found");

                existing.Status = false; // Đánh dấu đã hủy
                _unitOfWork.BookingRepository.Update(existing);
                await _unitOfWork.CommitAsync();

                return new ServiceResult(200, "Booking cancelled successfully");
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Error cancelling booking", new List<string> { ex.Message });
            }
        }
    }
}
