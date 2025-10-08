using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Common.DTOs.BookingDTO
{
    public class BookingDTO
    {
        public int BookingId { get; set; }
        public DateTime DateTime { get; set; }
        public string? Notes { get; set; }
        public bool? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int StationId { get; set; }
        public int VehicleId { get; set; }
        public int AccountId { get; set; }
    }
}
