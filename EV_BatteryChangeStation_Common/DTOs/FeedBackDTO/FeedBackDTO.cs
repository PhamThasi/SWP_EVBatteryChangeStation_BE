using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Common.DTOs.FeedBackDTO
{
    public class FeedBackDTO
    {
        public int FeedbackId { get; set; }
        public int? Rating { get; set; }
        public string Comment { get; set; }
        public DateTime? CreateDate { get; set; }
        public int AccountId { get; set; }
        public int BookingId { get; set; }
    }

    public class CreateFeedBackDTO
    {
        public int? Rating { get; set; }
        public string Comment { get; set; }
        public int AccountId { get; set; }
        public int BookingId { get; set; }
    }

    public class UpdateFeedBackDTO
    {
        public int? Rating { get; set; }
        public string? Comment { get; set; }
        public int? AccountId { get; set; }
        public int? BookingId { get; set; }
    }
}
