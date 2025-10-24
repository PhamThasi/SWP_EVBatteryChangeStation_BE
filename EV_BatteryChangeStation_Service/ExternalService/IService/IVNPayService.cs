using EV_BatteryChangeStation_Service.Base;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Service.ExternalService.IService
{
    public interface IVNPayService
    {
        Task<IServiceResult> CreatePaymentURL(Guid payment, string ipAddress);
        Task<IServiceResult> ValidateRespond(IQueryCollection queryParams);
    }
}
