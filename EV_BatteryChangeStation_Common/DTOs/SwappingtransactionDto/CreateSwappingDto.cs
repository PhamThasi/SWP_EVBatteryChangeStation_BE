using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Common.DTOs.SwappingtransactionDto
{
    public class CreateSwappingDto
    {
        public string? Notes { get; set; }

        public Guid StaffId { get; set; }

        public Guid VehicleId { get; set; }

        public Guid NewBatteryId { get; set; }
        public string Status { get; set; }

        public DateTime? CreateDate { get; set; }
    }
}
