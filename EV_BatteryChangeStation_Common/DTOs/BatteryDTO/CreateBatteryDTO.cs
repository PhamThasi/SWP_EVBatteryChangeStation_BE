using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Common.DTOs.BatteryDTO
{
    public class CreateBatteryDTO
    {
        public decimal? Capacity { get; set; }
        public bool? Status { get; set; } = true;
        public decimal? StateOfHealth { get; set; } = 100;
        public decimal? PercentUse { get; set; } = 0;
        public string TypeBattery { get; set; }
        public DateOnly? InsuranceDate { get; set; }
        public Guid StationId { get; set; }
    }
}
