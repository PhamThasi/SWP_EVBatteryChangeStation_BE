using EV_BatteryChangeStation_Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Repository.IRepositories
{
    public interface IBookingRepository
    {
        Task<List<Booking>> GetAllAsync();
        Task<Booking?> GetByIdAsync(Guid id);
        Task AddAsync(Booking booking);
        void Update(Booking booking);
        void Delete(Booking booking);
        Task<List<Booking>> GetByAccountIdAsync(Guid accountId);
        // Hiển<Task>: Thêm method để lấy booking theo StationId
        Task<List<Booking>> GetByStationIdAsync(Guid stationId);
    }
}
