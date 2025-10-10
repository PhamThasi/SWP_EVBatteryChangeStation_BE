using System;
using System.Collections.Generic;

namespace EV_BatteryChangeStation_Repository.Entities;

public partial class Payment
{
    public int PaymentId { get; set; }

    public decimal? Price { get; set; }

    public string? Method { get; set; }

    public bool? Status { get; set; }

    public DateTime? CreateDate { get; set; }

    public int? SubscriptionId { get; set; }

    public int? TransactionId { get; set; }

    public virtual Subscription? Subscription { get; set; }

    public virtual SwappingTransaction? Transaction { get; set; }
}
