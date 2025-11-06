using EV_BatteryChangeStation_Repository.Base;
using EV_BatteryChangeStation_Repository.Entities;  
using EV_BatteryChangeStation_Repository.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Repository.Repositories
{
    public class AccountRepository : GenericRepository<Account>, IAccountReporitory
    {
        public AccountRepository() {}

        public AccountRepository(EVBatterySwapContext context) => _context = context;

        public async Task<List<Account>> GetAccountByAccountName(string accountName)
        {
            return await _context.Accounts
                    .Include(r => r.Role)
                    .Where(a => a.AccountName.Contains(accountName.ToLower()))
                    .ToListAsync();
        }
        public async Task<Account> GetAccountByEmail(string email)
        {
            return await _context.Accounts.Include(r => r.Role)
                .FirstOrDefaultAsync(a => a.Email == email);
        }
        public async Task<Account> GetAccountByPhoneAsync(string phone)
        {
            return await _context.Accounts.Include(r => r.Role)
                .FirstOrDefaultAsync(a => a.PhoneNumber == phone);
        }
        public async Task<Account> GetAllAccount()
        {
            return await _context.Accounts.Include(r => r.Role)
                .FirstOrDefaultAsync();
        }
        public async Task<List<Account>> GetAllWithRoleAsync()
        {
            return await _context.Accounts.Include(r => r.Role)
                .ToListAsync();
        }
        public async Task<Account?> GetAllWithRoleAndStation(Guid id)
        {
            return await _context.Accounts
                .Include(r => r.Role)
                .Include(s => s.Station)
                .FirstOrDefaultAsync(a => a.AccountId == id);
        }

        public async Task<Account?> GetByAccountNameOrEmail(string keyword)
        {
            return await _context.Accounts
                .Include(r => r.Role)
                .FirstOrDefaultAsync(a => a.AccountName.ToLower() == keyword || a.Email.ToLower() == keyword);
        }

        public async Task<Account?> FindAsync(Expression<Func<Account, bool>> predicate)
        {
            return await _context.Accounts.FirstOrDefaultAsync(predicate);
        }
        public async Task<List<Account>> GetAllStaffAsync()
        {
            return await _context.Accounts
                .Include(r => r.Role)
                .Where(a => a.Role.RoleName == "Staff")
                .ToListAsync();
        }
    }
}
