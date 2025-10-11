using EV_BatteryChangeStation_Common.DTOs.FeedBackDTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Service.InternalService.IService
{
    public interface IFeedBackService
    {
        Task<List<FeedBackDTO>> GetAllAsync();
        Task<FeedBackDTO> GetByIdAsync(int id);
        Task<FeedBackDTO> CreateAsync(CreateFeedBackDTO dto);
        Task<FeedBackDTO> UpdateAsync(int id, UpdateFeedBackDTO dto);
        Task DeleteAsync(int id); // chỉ xóa cứng
    }
}
