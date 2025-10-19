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
        private static readonly HashidsNet.Hashids _hashids =
            new HashidsNet.Hashids("EV_BatteryChangeStation", 10);
        public static ViewSwappingDto MaptoEntity(this SwappingTransaction swappingTransaction)
        {
            if (swappingTransaction == null) throw new ArgumentNullException(nameof(swappingTransaction), "cannot be null");
            return new ViewSwappingDto
            {
                TransactionId = _hashids.Encode(swappingTransaction.TransactionId),
                Notes = swappingTransaction.Notes,
                StaffId = _hashids.Encode(swappingTransaction.StaffId),
                OldBatteryId = _hashids.Encode(swappingTransaction.OldBatteryId),
                NewBatteryId = _hashids.Encode(swappingTransaction.NewBatteryId),
                VehicleId = _hashids.Encode(swappingTransaction.VehicleId),
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
                StaffId = _hashids.DecodeSingle(createSwappingDto.StaffId),
                OldBatteryId = _hashids.DecodeSingle(createSwappingDto.OldBatteryId),
                NewBatteryId = _hashids.DecodeSingle(createSwappingDto.NewBatteryId),
                VehicleId = _hashids.DecodeSingle(createSwappingDto.VehicleId),
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
            if (updateSwappingDto.StaffId != null)
            {
                swappingTransaction.StaffId = _hashids.DecodeSingle(updateSwappingDto.StaffId);
            }
            if (updateSwappingDto.OldBatteryId != null)
            {
                swappingTransaction.OldBatteryId = _hashids.DecodeSingle(updateSwappingDto.OldBatteryId);
            }
            if (updateSwappingDto.NewBatteryId != null)
            {
                swappingTransaction.NewBatteryId = _hashids.DecodeSingle(updateSwappingDto.NewBatteryId);
            }
            if (updateSwappingDto.VehicleId != null)
            {
                swappingTransaction.VehicleId = _hashids.DecodeSingle(updateSwappingDto.VehicleId);
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
