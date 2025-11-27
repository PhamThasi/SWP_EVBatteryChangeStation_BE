using EV_BatteryChangeStation_Common.Enum.BookingEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Common.DTOs.BookingDTO
{
    public class BookingCreateDTO
    {
        public DateTime DateTime { get; set; }
        public string? Notes { get; set; }
        public string IsApproved { get; set; } 
        public DateTime? CreatedDate { get; set; }
        public Guid StationId { get; set; }
        public Guid VehicleId { get; set; }
        public Guid AccountId { get; set; }
        public Guid? BatteryId { get; set; }
    }

}
