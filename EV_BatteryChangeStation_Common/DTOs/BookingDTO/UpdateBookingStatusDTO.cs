using System;

namespace EV_BatteryChangeStation_Common.DTOs.BookingDTO
{
    /// <summary>
    /// DTO để cập nhật trạng thái booking (Approve/Reject)
    /// </summary>
    public class UpdateBookingStatusDTO
    {
        /// <summary>
        /// BookingId cần cập nhật
        /// </summary>
        public Guid BookingId { get; set; }

        /// <summary>
        /// Trạng thái mới: "Approved" hoặc "Rejected"
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Ghi chú (tùy chọn) - lý do approve/reject
        /// </summary>
        public string? Notes { get; set; }
    }
}

