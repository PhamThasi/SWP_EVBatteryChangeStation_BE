using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Common.DTOs.AuthencationDTO
{
    public class LoginRespondDTO
    {
        public Guid AccountId { get; set; }
        public string AccountName { get; set; } = null!;
        public string? Email { get; set; }
        public string? RoleName { get; set; }
    }
}
