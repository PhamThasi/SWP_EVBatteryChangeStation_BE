using System;

namespace EV_BatteryChangeStation_Common.DTOs.SwappingtransactionDto
{
    /// <summary>
    /// DTO để Staff xác nhận đổi pin sau khi payment thành công
    /// </summary>
    public class ConfirmSwapDTO
    {
        /// <summary>
        /// ID của Booking cần xác nhận đổi pin
        /// </summary>
        public Guid BookingId { get; set; }

        /// <summary>
        /// ID của Staff thực hiện đổi pin (lấy từ JWT Token)
        /// </summary>
        public Guid StaffId { get; set; }

        /// <summary>
        /// Ghi chú của Staff (tùy chọn)
        /// </summary>
        public string? Notes { get; set; }
    }
}

