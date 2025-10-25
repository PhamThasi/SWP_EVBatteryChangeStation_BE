using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Common.Enum.BookingEnum
{
    public enum BookingErrorCode
    {
        None = 0, // Không lỗi

        // ===== Input Validation =====
        MissingRequiredField = 1001,
        InvalidTimeRange = 1002,
        TimeInPast = 1003,
        DuplicateBooking = 1004,
        StationNotAvailable = 1005,
        VehicleNotFound = 1006,

        // ===== State & Logic =====
        BookingNotFound = 2001,
        BookingAlreadyCancelled = 2002,
        BookingAlreadyCompleted = 2003,
        InvalidStatusChange = 2004,
        PaymentFailed = 2005,
        RefundFailed = 2006,
        OverlappingBooking = 2007,

        // ===== System & Technical =====
        DatabaseError = 3001,
        TransactionFailed = 3002,
        UnexpectedError = 3999
    }
}
