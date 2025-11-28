using EV_BatteryChangeStation_Common.DTOs.RevenueDTO;
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
    public class RevenueRepository : IRevenueRepository
    {
        private readonly EVBatterySwapContext _context;

        public RevenueRepository(EVBatterySwapContext context)
        {
            _context = context;
        }
        public async Task<List<RevenueByStationDto>> GetRevenueRawAsync()
        {
            var totalRevenue = await _context.Payments
                .Where(p => p.Status == "Successful")
                .SumAsync(p => p.Price ?? 0);

            var totalTransaction = await _context.Payments
                .Where(p => p.Status == "Successful")
                .CountAsync();

            return new List<RevenueByStationDto>
    {
        new RevenueByStationDto
        {
            StationId = Guid.Empty,           // Không dùng station nữa
            StationName = "All System", // Tên mô tả hệ thống
            TotalRevenue = totalRevenue,
            TotalTransaction = totalTransaction.ToString()
        }
    };
        }
    }
}
