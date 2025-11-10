using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Common.DTOs.BookingDTO
{
    public class BookingDTO : BookingCreateDTO
    {
        public Guid BookingId { get; set; }
    }

}
