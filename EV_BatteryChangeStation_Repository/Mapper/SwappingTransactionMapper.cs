using EV_BatteryChangeStation_Common.DTOs.SwappingtransactionDto;
using EV_BatteryChangeStation_Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Repository.Mapper
{
    public static class SwappingTransactionMapper 
    {
        public static ViewSwappingDto MaptoEntity(this SwappingTransaction swappingTransaction)
        {
            if (swappingTransaction == null) throw new ArgumentNullException(nameof(swappingTransaction), "cannot be null");
            return new ViewSwappingDto
            {
                TransactionId = swappingTransaction.TransactionId,
                Notes = swappingTransaction.Notes,
                StaffId = swappingTransaction.StaffId,
                NewBatteryId = swappingTransaction.NewBatteryId,
                VehicleId = swappingTransaction.VehicleId,
                Status = swappingTransaction.Status,
                CreateDate = swappingTransaction.CreateDate
            };
        }

        public static SwappingTransaction MaptoCreate(this CreateSwappingDto createSwappingDto)
        {
            if (createSwappingDto == null) throw new ArgumentNullException(nameof(createSwappingDto), "cannot be null");
            return new SwappingTransaction
            {
                Notes = createSwappingDto.Notes,
                StaffId = createSwappingDto.StaffId,
                NewBatteryId = createSwappingDto.NewBatteryId,
                VehicleId = createSwappingDto.VehicleId,
                Status = createSwappingDto.Status,
                CreateDate = createSwappingDto.CreateDate
            };
        }

        public static void MaptoUpdate(this SwappingTransaction swappingTransaction, UpdateSwappingDto updateSwappingDto)
        {
            if (swappingTransaction == null) throw new ArgumentNullException(nameof(swappingTransaction), "cannot be null");
            if (updateSwappingDto == null) throw new ArgumentNullException(nameof(updateSwappingDto), "cannot be null");
            if (updateSwappingDto.Notes != null)
            {
                swappingTransaction.Notes = updateSwappingDto.Notes;
            }
            if (updateSwappingDto.StaffId != Guid.Empty)
            {
                swappingTransaction.StaffId = updateSwappingDto.StaffId;
            }
            if (updateSwappingDto.NewBatteryId != Guid.Empty)
            {
                swappingTransaction.NewBatteryId = updateSwappingDto.NewBatteryId;
            }
            if (updateSwappingDto.VehicleId != Guid.Empty)
            {
                swappingTransaction.VehicleId = updateSwappingDto.VehicleId;
            }
            if (updateSwappingDto.Status != null)
            {
                swappingTransaction.Status = updateSwappingDto.Status;
            }
            if (updateSwappingDto.CreateDate.HasValue)
            {
                swappingTransaction.CreateDate = updateSwappingDto.CreateDate;
            }
        }
    }
}
