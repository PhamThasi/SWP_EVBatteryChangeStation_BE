using EV_BatteryChangeStation_Common.DTOs.RevenueDTO;
using EV_BatteryChangeStation_Repository.UnitOfWork;
using EV_BatteryChangeStation_Service.InternalService.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Service.InternalService.Service
{
    public class RevenueService : IRevenueService
    {
        private readonly IUnitOfWork _unitOfWork;
        public RevenueService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<RevenueByStationDto>> GetRevenueByStationAsync()
        {
            return await _unitOfWork.RevenueRepository.GetRevenueRawAsync();
        }
    }
}


