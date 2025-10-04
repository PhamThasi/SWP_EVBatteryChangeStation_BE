using EV_BatteryChangeStation_Common.DTOs.StationDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Service.InternalService.IService
{
    public interface IStationService
    {
        public Task<IEnumerable<StationDTO>> GetAllAsync();
        public Task<StationDTO> GetByIdAsync(int id);
        public Task<StationDTO> CreateAsync(StationDTO dto);
        public Task<bool> UpdateAsync(StationDTO dto);
        public Task<bool> DeleteAsync(int id);
    }
}
