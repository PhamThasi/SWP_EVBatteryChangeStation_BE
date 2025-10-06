using EV_BatteryChangeStation_Repository.Entities;
using EV_BatteryChangeStation_Repository.IRepositories;
using EV_BatteryChangeStation_Repository.Repositories;

namespace EV_BatteryChangeStation_Repository.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EVBatterySwapContext _context;

        private IAccountReporitory _accountRepository;
        private IRoleRepository _roleRepository;
        public UnitOfWork(EVBatterySwapContext context)
        {
            _context = context;
        }

        public IAccountReporitory AccountRepository
        {
            get
            {
                return _accountRepository ??= new AccountRepository(_context);
            }
        }
        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public IRoleRepository RoleRepository
        {
            get
            {
                return _roleRepository ??= new RoleRepository(_context);
            }
        }
    }
}
