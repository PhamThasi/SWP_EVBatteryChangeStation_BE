using EV_BatteryChangeStation_Repository.DBContext;
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
        private IStationRepository _stationRepository;
        private BookingRepository _bookingRepository;
        private IBatteryRepository _batteryRepository;
        private ICarRepository _carRepository;

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
        public IStationRepository StationRepository => 
            _stationRepository ??= new StationRepository(_context);

        public IBookingRepository BookingRepository
        {
            get
            {
                return _bookingRepository ??= new BookingRepository(_context);
            }
        }
        public IBatteryRepository BatteryRepository
        {
            get
            {
                return _batteryRepository ??= new BatteryRepository(_context);
            }
        }
        public ICarRepository CarRepository
        {
            get
            {
                return _carRepository ??= new CarRepository(_context);
            }
        }
    }
}
