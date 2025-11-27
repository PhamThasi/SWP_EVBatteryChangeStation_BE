using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Common.Enum.BookingEnum
{
    public enum BookingApprovalStatus
    {
        Pending,    // 0 - Đang chờ duyệt
        Approved,   // 1 - Đã duyệt
        Rejected,   // 2 - Bị từ chối
        Canceled,   // 3 - Đã huỷ
        Completed  // 4 - Đã hoàn thành (đã đổi pin)
    }
}
