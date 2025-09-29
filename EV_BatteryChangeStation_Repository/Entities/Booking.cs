using System;
using System.Collections.Generic;

namespace EV_BatteryChangeStation_Repository.Entities;

public partial class Booking
{
    public int BookingId { get; set; }

    public DateTime DateTime { get; set; }

    public string? Notes { get; set; }

    public bool? Status { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int StationId { get; set; }

    public int VehicleId { get; set; }

    public int AccountId { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual Station Station { get; set; } = null!;

    public virtual Car Vehicle { get; set; } = null!;
}
