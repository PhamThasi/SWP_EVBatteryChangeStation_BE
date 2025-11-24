using EV_BatteryChangeStation_Repository.Entities;
using EV_BatteryChangeStation_Repository.IRepositories;
using EV_BatteryChangeStation_Repository.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace EV_BatteryChangeStation_Repository.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EVBatterySwapContext _context;

        private IAccountReporitory _accountRepository;
        private IRoleRepository _roleRepository;
        private IStationRepository _stationRepository;
        private IBookingRepository _bookingRepository;
        private IFeedBackRepository _feedBackRepository;
        private IBatteryRepository _batteryRepository;
        private ICarRepository _carRepository;
        private ISubscriptionRepository _subscriptionRepository;
        private ISupportRequestRepository _supportRequestRepository;
        private ISwappingTransactionRepository _swappingTransactionRepository;
        private IPaymentRepository _paymentRepository;
        private IRevenueRepository _revenueRepository;

        public UnitOfWork(EVBatterySwapContext context)
        {
            _context = context;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
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

        public IFeedBackRepository FeedBackRepository =>
    _feedBackRepository ??= new FeedBackRepository(_context);
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

        public ISubscriptionRepository SubscriptionRepository
        {
            get
            {
                return _subscriptionRepository ??= new SubscriptionRepository(_context);
            }
        }
        public ISupportRequestRepository SupportRequestRepository
        {
            get
            {
                return _supportRequestRepository ??= new SupportRequestRepository(_context);
            }
        }
        public ISwappingTransactionRepository SwappingTransactionRepository
        {
            get
            {
                return _swappingTransactionRepository ??= new SwappingTransactionRepository(_context);
            }
        }
        public IPaymentRepository PaymentRepository
        {
            get
            {
                return _paymentRepository ??= new PaymentRepository(_context);
            }
        }

        public IRevenueRepository RevenueRepository
        {
            get
            {
                return _revenueRepository ??= new RevenueRepository(_context);
            }
        }
    }
}