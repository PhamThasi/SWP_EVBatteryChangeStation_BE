using EV_BatteryChangeStation_Common.DTOs.BookingDTO;
using EV_BatteryChangeStation_Service.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Service.InternalService.IService
{
    public interface IBookingService
    {
        Task<ServiceResult> GetAllAsync();
        Task<ServiceResult> GetByIdAsync(Guid id);
        Task<ServiceResult> CreateAsync(BookingCreateDTO dto);
        Task<ServiceResult> UpdateAsync(Guid id, BookingCreateDTO dto);
        Task<ServiceResult> DeleteAsync(Guid id);
        Task<ServiceResult> HardDeleteAsync(Guid id);
        Task<ServiceResult> GetByAccountIdAsync(Guid accountId);
        // Hiển<Task>: Thêm method để Staff xem booking của Station mà họ đang làm việc
        Task<ServiceResult> GetByStaffStationAsync(Guid staffAccountId);
        
        /// <summary>
        /// Staff xác nhận hoặc từ chối booking (chuyển trạng thái Pending → Approved/Rejected)
        /// </summary>
        Task<ServiceResult> UpdateBookingStatusAsync(Guid bookingId, string status, Guid staffId, string? notes = null);
    }
}
