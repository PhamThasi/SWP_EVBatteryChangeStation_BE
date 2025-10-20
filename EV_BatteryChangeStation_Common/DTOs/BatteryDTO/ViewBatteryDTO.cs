using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Common.DTOs.BatteryDTO
{
    public class ViewBatteryDTO
    {
        public Guid BatteryId { get; set; }
        public decimal? Capacity { get; set; }
        public DateTime? LastUsed { get; set; }
        public bool? Status { get; set; }
        public decimal? StateOfHealth { get; set; }
        public decimal? PercentUse { get; set; }
        public string TypeBattery { get; set; }
        public DateTime? BatterySwapDate { get; set; }
        public DateOnly? InsuranceDate { get; set; }
        public Guid StationId { get; set; }
    }
}
