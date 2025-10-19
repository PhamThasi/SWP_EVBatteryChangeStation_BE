using EV_BatteryChangeStation_Common.DTOs.SwappingtransactionDto;
using EV_BatteryChangeStation_Service.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Service.InternalService.IService
{
    public interface ISwappingService
    {
        Task<IServiceResult> GetAllTransactionsAsync();
        Task<IServiceResult> GetTransactionByIdAsync(string transactionId);
        Task<IServiceResult> CreateTransactionAsync(CreateSwappingDto createSwappingDto);
        Task<IServiceResult> UpdateTransactionAsync(UpdateSwappingDto updateSwappingDto);
        Task<IServiceResult> DeleteTransactionAsync(string transactionId);
        Task<IServiceResult> SoftDeleteTransactionAsync(string transactionid);
        Task<IServiceResult> GetTransactionByCarIdAsync(string carid);
    }
}
