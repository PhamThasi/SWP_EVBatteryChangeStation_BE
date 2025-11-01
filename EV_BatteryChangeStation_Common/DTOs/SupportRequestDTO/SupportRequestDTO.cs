using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Common.DTOs.SupportRequestDTO
{
    //  Dùng cho API Create
    public class SupportRequestCreateDTO
    {
        public string? IssueType { get; set; }
        public string? Description { get; set; }
        public Guid AccountId { get; set; }
        public string? ResponseText { get; set; }
    }

    //  Dùng cho API Update
    public class SupportRequestUpdateDTO
    {
        public string? IssueType { get; set; }
        public string? Description { get; set; }
        public Guid? StaffId { get; set; }
        public string? ResponseText { get; set; }
    }

    //  DTO để hiển thị
    public class SupportRequestViewDTO
    {
        public Guid RequestId { get; set; }
        public string? IssueType { get; set; }
        public string? Description { get; set; }
        public DateTime? CreateDate { get; set; }
        public bool? Status { get; set; }
        public Guid AccountId { get; set; }
        public Guid? StaffId { get; set; }
        public string? ResponseText { get; set; }
        public DateTime? ResponseDate { get; set; }
    }
}
