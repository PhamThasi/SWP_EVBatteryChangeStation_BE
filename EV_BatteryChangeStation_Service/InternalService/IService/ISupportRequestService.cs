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
        Task<ServiceResult> GetByIdAsync(int id);
        Task<ServiceResult> CreateAsync(SupportRequestCreateUpdateDTO dto);
        Task<ServiceResult> UpdateAsync(int id, SupportRequestCreateUpdateDTO dto);
        Task<ServiceResult> SoftDeleteAsync(int id);
        Task<ServiceResult> HardDeleteAsync(int id);
    }
}
