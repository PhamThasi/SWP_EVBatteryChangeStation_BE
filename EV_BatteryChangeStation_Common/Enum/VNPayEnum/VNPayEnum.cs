using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Common.Enum.VNPayEnum
{
    public static class VNPayEnum
    {
        public const string SUCCESS = "SUCCESS";
        public const string FAILED = "FAILED";
        public const string PENDING = "PENDING";
        public const string CANCELLED = "CANCELLED";
    }
    public static class Method
    {
        public const string VNPAY = "VNPAY";
        public const string CASH = "CASH";
    }
}
