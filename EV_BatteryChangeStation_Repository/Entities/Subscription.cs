using System;
using System.Collections.Generic;

namespace EV_BatteryChangeStation_Repository.Entities;

public partial class Subscription
{
    public int SubscriptionId { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public decimal? ExtraFee { get; set; }

    public string? Description { get; set; }

    public int? DurationPackage { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public int AccountId { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual Payment? Payment { get; set; }
}
