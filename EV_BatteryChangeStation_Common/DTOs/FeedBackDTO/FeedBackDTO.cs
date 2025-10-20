using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Common.DTOs.FeedBackDTO
{
    public class FeedBackDTO
    {
        public Guid FeedbackId { get; set; }
        public int? Rating { get; set; }
        public string Comment { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid AccountId { get; set; }
        public Guid BookingId { get; set; }
    }

    public class CreateFeedBackDTO
    {
        public int? Rating { get; set; }
        public string Comment { get; set; }
        public Guid AccountId { get; set; }
        public Guid BookingId { get; set; }
    }

    public class UpdateFeedBackDTO
    {
        public int? Rating { get; set; }
        public string? Comment { get; set; }
        public Guid? AccountId { get; set; }
        public Guid? BookingId { get; set; }
    }
}
