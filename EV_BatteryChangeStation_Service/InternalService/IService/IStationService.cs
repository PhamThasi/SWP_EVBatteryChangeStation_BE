using EV_BatteryChangeStation_Common.DTOs.StationDTO;
using EV_BatteryChangeStation_Service.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Service.InternalService.IService
{
    public interface IStationService
    {
        public Task<ServiceResult> GetAllAsync();
        public Task<ServiceResult> GetByIdAsync(int id);
        public Task<ServiceResult> CreateAsync(StationCreateDTO dto);
        public Task<ServiceResult> UpdateAsync(int id, StationCreateDTO dto);
        public Task<ServiceResult> DeleteAsync(int id);
        public Task<ServiceResult> HardDeleteAsync(int id);
    }
}
