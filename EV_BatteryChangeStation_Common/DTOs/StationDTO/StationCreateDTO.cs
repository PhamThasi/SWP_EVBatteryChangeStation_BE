using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Common.DTOs.StationDTO
{
    public class StationCreateDTO
    {
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public bool? Status { get; set; }
        public string? StationName { get; set; }
        public int? BatteryQuantity { get; set; }
    }
}
