using EV_BatteryChangeStation_Common.DTOs.SupportRequestDTO;
using EV_BatteryChangeStation_Service.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Service.InternalService.IService
{
    public interface ISupportRequestService
    {
        Task<ServiceResult> GetAllAsync();
        Task<ServiceResult> GetByIdAsync(Guid id);
        Task<ServiceResult> CreateAsync(SupportRequestCreateUpdateDTO dto);
        Task<ServiceResult> UpdateAsync(Guid id, SupportRequestCreateUpdateDTO dto);
        Task<ServiceResult> SoftDeleteAsync(Guid id);
        Task<ServiceResult> HardDeleteAsync(Guid id);
    }
}
