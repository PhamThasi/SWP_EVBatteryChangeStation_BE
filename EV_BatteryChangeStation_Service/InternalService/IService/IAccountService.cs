using EV_BatteryChangeStation_Common.DTOs.AccountDto;
using EV_BatteryChangeStation_Service.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Service.InternalService.IService
{
    public interface IAccountService
    {
        Task<IServiceResult> CreateAccountAsync(CreateAccountDTO createAccount);
        Task<IServiceResult> UpdateAccountAsync(UpdateAccountDTO updateAccount);
        Task<IServiceResult> DeleteAccountAsync(Guid Id);
        Task<IServiceResult> GetAllAccountsAsync();
        Task<IServiceResult> GetAccountByNameAsync(string accountName);
        //Task<IServiceResult> GetAllAccountWithIdDecodeAsync();
        Task<IServiceResult> SoftDeleteAsync(Guid Id);
        Task<IServiceResult> GetAllStaffAccountAsync();
    }
}
