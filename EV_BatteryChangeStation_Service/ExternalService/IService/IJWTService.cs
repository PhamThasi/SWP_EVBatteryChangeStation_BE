using EV_BatteryChangeStation_Common.DTOs.AuthencationDTO;
using EV_BatteryChangeStation_Service.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Service.ExternalService.IService
{
    public interface IJWTService 
    {
        string GenerateToken(LoginRespondDTO tokenDto);
        Task<IServiceResult> ValidateToken(string token);
    }
}
