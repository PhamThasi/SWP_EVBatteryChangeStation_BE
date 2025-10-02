using EV_BatteryChangeStation_Repository.DBContext;
using EV_BatteryChangeStation_Repository.IRepositories;
using EV_BatteryChangeStation_Repository.Repositories;

namespace EV_BatteryChangeStation_Repository.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EvbatterySwapContext _context;

        private IAccountReporitory _accountRepository;
        private IRoleRepository _roleRepository;
        public UnitOfWork(EvbatterySwapContext context)
        {
            _context = context;
        }

        public UnitOfWork()
        {
            _context = new EvbatterySwapContext();
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
