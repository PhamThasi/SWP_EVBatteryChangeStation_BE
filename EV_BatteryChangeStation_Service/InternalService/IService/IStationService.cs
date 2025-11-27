using EV_BatteryChangeStation_Common.DTOs.StationDTO;
using EV_BatteryChangeStation_Service.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Service.InternalService.IService
{
    public interface IStationService
    {
        public Task<ServiceResult> GetAllAsync();
        public Task<ServiceResult> GetByIdAsync(Guid id);
        public Task<ServiceResult> CreateAsync(StationCreateDTO dto);
        public Task<ServiceResult> UpdateAsync(Guid id, StationCreateDTO dto);
        public Task<ServiceResult> DeleteAsync(Guid id);
        public Task<ServiceResult> HardDeleteAsync(Guid id);
        public Task<ServiceResult> SearchByNameAsync(string keyword);
        public Task<ServiceResult> GetByNameAsync(string name);
    }
}
