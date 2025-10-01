using System;
using System.Collections.Generic;

namespace EV_BatteryChangeStation_Repository.Entities;

public partial class SupportRequest
{
    public int RequestId { get; set; }

    public string? IssueType { get; set; }

    public string? Description { get; set; }

    public DateTime? CreateDate { get; set; }

    public bool? Status { get; set; }

    public int AccountId { get; set; }

    public int? StaffId { get; set; }

    public string? ResponseText { get; set; }

    public DateTime? ResponseDate { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual Account? Staff { get; set; }
}
