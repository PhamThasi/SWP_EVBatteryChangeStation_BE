using EV_BatteryChangeStation_Repository.Base;
using EV_BatteryChangeStation_Repository.Entities;
using EV_BatteryChangeStation_Repository.IRepositories;

namespace EV_BatteryChangeStation_Repository.Repositories
{
    public class SupportRequestRepository : GenericRepository<SupportRequest>, ISupportRequestRepository
    {
        public SupportRequestRepository(EVBatterySwapContext context) : base(context)
        {
        }
    }
}
