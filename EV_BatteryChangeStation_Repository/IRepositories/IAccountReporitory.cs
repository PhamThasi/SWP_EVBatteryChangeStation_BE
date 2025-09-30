using EV_BatteryChangeStation_Repository.Base;
using EV_BatteryChangeStation_Repository.Entities;


namespace EV_BatteryChangeStation_Repository.IRepositories
{
    public interface IAccountReporitory : IGenericRepository<Account>
    {
        Task<Account> GetAccountByAccountName(string accountName);
        Task<Account> GetAccountByEmail(string email);
        Task<Account> GetAccountByPhoneAsync(string phone);
        Task<Account> GetAllAccount();
        Task<List<Account>> GetAllWithRoleAsync();
        Task<Account?> GetAllWithRoleAndStation(int id);
        Task<Account?> GetByAccountNameOrEmail(string keyword);
    }
}
