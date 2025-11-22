using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Common.DTOs.CarDTO
{
    public class ViewCarDTO
    {
        public Guid VehicleId { get; set; }

        public string Model { get; set; }

        public string BatteryType { get; set; }

        public string Producer { get; set; }

        public DateTime? CreateDate { get; set; }

        public string Images { get; set; }

        public string Status { get; set; } = null!;
    }
}
