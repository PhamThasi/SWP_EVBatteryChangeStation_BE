using System;

namespace EV_BatteryChangeStation_Common.DTOs.PaymentDTO
{
    /// <summary>
    /// DTO để check subscription status - dùng để quyết định có cần redirect đến trang thanh toán hay không
    /// Dựa vào payment thành công có subscription còn hạn
    /// </summary>
    public class SubscriptionStatusCheckDto
    {
        /// <summary>
        /// User có payment thành công với subscription đang active và còn hạn không
        /// </summary>
        public bool HasActiveSubscription { get; set; }

        /// <summary>
        /// Có cần redirect đến trang thanh toán không (false nếu đã có subscription active)
        /// </summary>
        public bool NeedsRedirect { get; set; }

        /// <summary>
        /// Thông tin payment nếu có (null nếu không có)
        /// </summary>
        public PaymentRespondDto? Payment { get; set; }
    }
}

