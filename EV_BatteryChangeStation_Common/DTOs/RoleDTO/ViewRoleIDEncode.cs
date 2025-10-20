using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Common.DTOs.RoleDTO
{
    public class ViewRoleIDEncode
    {
        public Guid RoleId { get; set; }
        public string RoleName { get; set; } = null!;
    }
}
