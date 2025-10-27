using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Common.DTOs.SubscriptionDTO
{
    public class SubscriptionCreateUpdateDTO
    {
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public decimal? ExtraFee { get; set; }
        public string? Description { get; set; }
        public int? DurationPackage { get; set; }
        public Guid? AccountId { get; set; }
    }
    public class SubscriptionViewDTO
    {
        public Guid SubscriptionId { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public decimal? ExtraFee { get; set; }
        public string? Description { get; set; }
        public int? DurationPackage { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? AccountId { get; set; }
    }
}
