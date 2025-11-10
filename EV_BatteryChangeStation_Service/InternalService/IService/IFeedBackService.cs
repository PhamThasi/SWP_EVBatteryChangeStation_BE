using EV_BatteryChangeStation_Common.DTOs.FeedBackDTO;
using EV_BatteryChangeStation_Service.Base;
using System;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Service.InternalService.IService
{
    public interface IFeedBackService
    {
        Task<ServiceResult> GetAllAsync();
        Task<ServiceResult> GetByIdAsync(Guid id);
        Task<ServiceResult> CreateAsync(CreateFeedBackDTO dto);
        Task<ServiceResult> UpdateAsync(Guid id, UpdateFeedBackDTO dto);
        Task<ServiceResult> DeleteAsync(Guid id);
        Task<ServiceResult> GetByAccountIdAsync(Guid accountId);

    }
}
