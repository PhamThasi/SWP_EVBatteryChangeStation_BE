using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Common.DTOs.SwappingtransactionDto
{
    public class UpdateSwappingDto
    {
        public string TransactionId { get; set; }
        public string? Notes { get; set; }

        public string StaffId { get; set; }

        public string OldBatteryId { get; set; }

        public string VehicleId { get; set; }

        public string NewBatteryId { get; set; }
        public string Status { get; set; }

        public DateTime? CreateDate { get; set; }
    }
}
