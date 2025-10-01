using System;
using System.Collections.Generic;

namespace EV_BatteryChangeStation_Repository.Entities;

public partial class SwappingTransaction
{
    public int TransactionId { get; set; }

    public string? Notes { get; set; }

    public int StaffId { get; set; }

    public int OldBatteryId { get; set; }

    public int VehicleId { get; set; }

    public int NewBatteryId { get; set; }

    public DateTime? CreateDate { get; set; }

    public virtual Battery NewBattery { get; set; } = null!;

    public virtual Payment? Payment { get; set; }

    public virtual Account Staff { get; set; } = null!;

    public virtual Car Vehicle { get; set; } = null!;
}
