using EV_BatteryChangeStation_Common.DTOs.FeedBackDTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Service.InternalService.IService
{
    public interface IFeedBackService
    {
        Task<List<FeedBackDTO>> GetAllAsync();
        Task<FeedBackDTO> GetByIdAsync(Guid id);
        Task<FeedBackDTO> CreateAsync(CreateFeedBackDTO dto);
        Task<FeedBackDTO> UpdateAsync(Guid id, UpdateFeedBackDTO dto);
        Task DeleteAsync(Guid id); // chỉ xóa cứng
    }
}
