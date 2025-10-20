using EV_BatteryChangeStation_Common.DTOs.RoleDTO;
using EV_BatteryChangeStation_Service.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Service.InternalService.IService
{
    public interface IRoleService
    {
        Task<IServiceResult> CreateRoleAsync(CreateRoleDTO createRole);
        Task<IServiceResult> UpdateRoleAsync(UpdateRoleDTO updateRole);
        Task<IServiceResult> DeleteRoleAsync(Guid deleteid);
        Task<IServiceResult> SoftDeleteAsync(Guid softId);
        Task<IServiceResult> GetAllRolesAsync();
        Task<IServiceResult> GetRoleByNameAsync(string roleName);
        //Task<IServiceResult> GetAllRoleByIdDecodeAsync();
    }
}
