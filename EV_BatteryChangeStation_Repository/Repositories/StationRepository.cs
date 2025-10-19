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
    public class StationRepository : IStationRepository
    {
        private readonly EVBatterySwapContext _context;

        public StationRepository(EVBatterySwapContext context)
        {
            _context = context;
        }

        public async Task<List<Station>> GetAllAsync() =>
            await _context.Stations.ToListAsync();

        public async Task<Station> GetByIdAsync(int id) =>
            await _context.Stations.FindAsync(id);

        public void Create(Station station) =>
            _context.Stations.Add(station);

        public void Update(Station station) =>
            _context.Stations.Update(station);

        public void Delete(Station station) =>
            _context.Stations.Remove(station);
    }
}
