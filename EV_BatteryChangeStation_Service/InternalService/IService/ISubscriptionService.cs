using EV_BatteryChangeStation_Common.DTOs.SubscriptionDTO;
using EV_BatteryChangeStation_Service.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Service.InternalService.IService
{
    public interface ISubscriptionService
    {
        Task<ServiceResult> GetAllAsync();
        Task<ServiceResult> GetByIdAsync(int id);
        Task<ServiceResult> CreateAsync(SubscriptionCreateUpdateDTO dto);
        Task<ServiceResult> UpdateAsync(int id, SubscriptionCreateUpdateDTO dto);
        Task<ServiceResult> SoftDeleteAsync(int id);
        Task<ServiceResult> HardDeleteAsync(int id);
    }
}
