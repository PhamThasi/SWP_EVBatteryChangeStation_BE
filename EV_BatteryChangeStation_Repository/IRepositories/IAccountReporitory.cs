using EV_BatteryChangeStation_Repository.Base;
using EV_BatteryChangeStation_Repository.Entities;
using System.Linq.Expressions;


namespace EV_BatteryChangeStation_Repository.IRepositories
{
    public interface IAccountReporitory : IGenericRepository<Account>
    {
        Task<List<Account>> GetAccountByAccountName(string accountName);
        Task<Account> GetAccountByEmail(string email);
        Task<Account> GetAccountByPhoneAsync(string phone);
        Task<Account> GetAllAccount();
        Task<List<Account>> GetAllWithRoleAsync();
        Task<Account?> GetAllWithRoleAndStation(Guid id);
        Task<Account?> GetByAccountNameOrEmail(string keyword);
        Task<Account?> FindAsync(Expression<Func<Account, bool>> predicate);
        Task<List<Account>> GetAllStaffAsync();
    }
}
