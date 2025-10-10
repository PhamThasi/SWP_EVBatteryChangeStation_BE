using System;
using System.Collections.Generic;

namespace EV_BatteryChangeStation_Repository.Entities;

public partial class Feedback
{
    public int FeedbackId { get; set; }

    public int? Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime? CreateDate { get; set; }

    public int AccountId { get; set; }

    public int BookingId { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual Booking Booking { get; set; } = null!;
}
