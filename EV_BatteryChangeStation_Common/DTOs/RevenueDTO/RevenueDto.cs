using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Common.DTOs.RevenueDTO
{
    public class RevenueByStationDto
    {
        public Guid StationId { get; set; }
        public string StationName { get; set; }
        public decimal TotalRevenue { get; set; }
        public string TotalTransaction { get; set; }

    }
}
