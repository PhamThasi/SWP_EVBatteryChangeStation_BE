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
                    var data = await _context.Payments
             .Where(p => p.Status == "Successful")
             .Join(_context.SwappingTransactions,
                   p => p.TransactionId,
                   s => s.TransactionId,
                   (p, s) => new { p, s })
             .Join(_context.Batteries,
                   ps => ps.s.NewBatteryId,
                   b => b.BatteryId,
                   (ps, b) => new { ps.p, b.StationId })
             .Join(_context.Stations,
                   pb => pb.StationId,
                   st => st.StationId,
                   (pb, st) => new
                   {
                       st.StationId,
                       st.Address,
                       pb.p.Price
                   })
             .GroupBy(x => new { x.StationId, x.Address })
             .Select(g => new RevenueByStationDto
             {
                 StationId = g.Key.StationId,
                 StationName = g.Key.Address,
                 TotalRevenue = g.Sum(x => x.Price) ?? 0,
                 TotalTransaction = g.Count().ToString()
             })
             .ToListAsync();
            return data;
        }
    }
}
