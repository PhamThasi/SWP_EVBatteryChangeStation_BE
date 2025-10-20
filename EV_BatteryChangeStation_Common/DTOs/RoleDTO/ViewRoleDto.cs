using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Common.DTOs.RoleDTO
{
    public class ViewRoleDto
    {
        public Guid RoleId { get; set; }
        public string RoleName { get; set; } = null!;

        public bool Status { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }
    }
}
