using EV_BatteryChangeStation_Common.DTOs.RoleDTO;
using EV_BatteryChangeStation_Repository.Entities;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;

namespace EV_BatteryChangeStation_Repository.Mapper
{
    public static class RoleMapper
    {
        public static Role toModel(this CreateRoleDTO createRole)
        {
            if (createRole == null)
            {
                throw new ArgumentNullException(nameof(createRole));
            }
            return new Role
            {
                RoleName = createRole.RoleName,
                Status = createRole.Status
            };
        }

        public static ViewRoleDto MaptoViewRoleDto(this Role role)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            return new ViewRoleDto
            {
                RoleId = role.RoleId,
                RoleName = role.RoleName,
                Status = role.Status,
                CreateDate = role.CreateDate,
                UpdateDate = role.UpdateDate
            };
        }

        public static void ToUpdateRoleFromDTO(this Role role, UpdateRoleDTO updateRole)
        {
            if (updateRole == null) return;
            if (!string.IsNullOrEmpty(updateRole.RoleName))
            {
                role.RoleName = updateRole.RoleName;
            }
            if (updateRole.Status.HasValue)
            {
                role.Status = updateRole.Status.Value;  
            }
        }
    }
}
