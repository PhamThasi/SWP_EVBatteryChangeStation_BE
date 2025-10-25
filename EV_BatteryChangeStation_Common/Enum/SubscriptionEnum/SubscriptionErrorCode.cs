using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Common.Enum.SubscriptionEnum
{
    public enum SubscriptionErrorCode
    {
        None = 0, // Không lỗi

        // ===== Input Validation =====
        MissingRequiredField = 1001,
        InvalidDateRange = 1002,
        InvalidPrice = 1003,

        // ===== Logic & State =====
        SubscriptionNotFound = 2001,
        SubscriptionAlreadyExpired = 2002,
        SubscriptionAlreadyCancelled = 2003,
        PaymentPending = 2004,
        RenewalFailed = 2005,
        UpgradeNotAllowed = 2006,
        DowngradeNotAllowed = 2007,
        DuplicateActiveSubscription = 2008,

        // ===== System =====
        DatabaseError = 3001,
        TransactionFailed = 3002,
        UnexpectedError = 3999
    }
}
