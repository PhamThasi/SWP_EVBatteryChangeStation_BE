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
        Task<ServiceResult> GetByIdAsync(Guid id);
        Task<ServiceResult> CreateAsync(SubscriptionCreateUpdateDTO dto);
        Task<ServiceResult> UpdateAsync(Guid id, SubscriptionCreateUpdateDTO dto);
        Task<ServiceResult> SoftDeleteAsync(Guid id);
        Task<ServiceResult> HardDeleteAsync(Guid id);
        Task<ServiceResult> RestoreAsync(Guid id);
        /// <summary>
        /// Lấy subscription đang active của user (nếu có)
        /// </summary>
        Task<ServiceResult> GetActiveByAccountIdAsync(Guid accountId);
    }
}
