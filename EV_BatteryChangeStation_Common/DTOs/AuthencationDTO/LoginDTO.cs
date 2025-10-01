using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Common.DTOs.AuthencationDTO
{
    public class LoginDTO
    {
        public string? Keyword { get; set; }
        public string? Password { get; set; }
    }
}
