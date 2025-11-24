using EV_BatteryChangeStation_Repository.IRepositories;
using Microsoft.EntityFrameworkCore.Storage;


namespace EV_BatteryChangeStation_Repository.UnitOfWork
{
    public interface IUnitOfWork
    {
        IAccountReporitory AccountRepository { get; }
        IRoleRepository RoleRepository { get; }
        IStationRepository StationRepository { get; }
        IBookingRepository BookingRepository { get; }
        IFeedBackRepository FeedBackRepository { get; }
        Task<int> CommitAsync();
        IBatteryRepository BatteryRepository { get; }
        ICarRepository CarRepository { get; }
        ISubscriptionRepository SubscriptionRepository { get; }
        ISupportRequestRepository SupportRequestRepository { get; }
        ISwappingTransactionRepository SwappingTransactionRepository { get; }
        IPaymentRepository PaymentRepository { get; }
        IRevenueRepository RevenueRepository { get; }
        Task<IDbContextTransaction> BeginTransactionAsync();

    }
}
