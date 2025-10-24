using EV_BatteryChangeStation_Repository.Base;
using EV_BatteryChangeStation_Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Repository.IRepositories
{
    public interface ISwappingTransactionRepository : IGenericRepository<SwappingTransaction>
    {
        Task<List<SwappingTransaction>> GetAllSwappingTransactionDetail();
        Task<List<SwappingTransaction>> getByCarId(Guid carId);
    }
}
