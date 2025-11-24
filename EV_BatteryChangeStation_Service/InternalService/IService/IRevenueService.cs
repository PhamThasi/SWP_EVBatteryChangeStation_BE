using EV_BatteryChangeStation_Common.DTOs.RevenueDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Service.InternalService.IService
{
    public interface IRevenueService
    {
        Task<List<RevenueByStationDto>> GetRevenueByStationAsync();
    }
}
