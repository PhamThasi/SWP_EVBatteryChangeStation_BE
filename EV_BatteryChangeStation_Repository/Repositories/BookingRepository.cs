using EV_BatteryChangeStation_Repository.Entities;
using EV_BatteryChangeStation_Repository.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Repository.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly EVBatterySwapContext _context;

        public BookingRepository(EVBatterySwapContext context)
        {
            _context = context;
        }

        public async Task<List<Booking>> GetAllAsync()
        {
            return await _context.Bookings
                .Include(b => b.Station)
                .Include(b => b.Vehicle)
                .Include(b => b.Account)
                .Include(b => b.Battery)
                .ToListAsync();
        }

        public async Task<Booking?> GetByIdAsync(Guid id)
        {
            return await _context.Bookings
                .Include(b => b.Station)
                .Include(b => b.Vehicle)
                .Include(b => b.Account)
                .Include(b => b.Battery)
                .FirstOrDefaultAsync(b => b.BookingId == id);
        }

        public async Task AddAsync(Booking booking)
        {
            await _context.Bookings.AddAsync(booking);
        }

        public void Update(Booking booking)
        {
            _context.Bookings.Update(booking);
        }

        public void Delete(Booking booking)
        {
            _context.Bookings.Remove(booking);
        }

        public async Task<List<Booking>> GetByAccountIdAsync(Guid accountId)
        {
            return await _context.Bookings
                .Where(b => b.AccountId == accountId)
                .ToListAsync();
        }

        // Hiển<Task>: Lấy danh sách booking theo StationId, có Include Station, Vehicle, Account và sắp xếp theo DateTime
        public async Task<List<Booking>> GetByStationIdAsync(Guid stationId)
        {
            return await _context.Bookings
                .Where(b => b.StationId == stationId)
                .Include(b => b.Station)
                .Include(b => b.Vehicle)
                .Include(b => b.Account)
                .Include(b => b.Battery)
                .OrderByDescending(b => b.DateTime)
                .ToListAsync();
        }
    }
}
