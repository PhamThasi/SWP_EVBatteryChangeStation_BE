using EV_BatteryChangeStation_Common.DTOs.BookingDTO;
using EV_BatteryChangeStation_Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Repository.Mapper
{
    public static class BookingMapper
    {
        public static BookingDTO ToDTO(Booking entity)
        {
            if (entity == null) return null;
            return new BookingDTO
            {
                BookingId = entity.BookingId,
                DateTime = entity.DateTime,
                Notes = entity.Notes,
                Status = entity.Status,
                CreatedDate = entity.CreatedDate,
                StationId = entity.StationId,
                VehicleId = entity.VehicleId,
                AccountId = entity.AccountId
            };
        }

        public static Booking ToEntity(BookingDTO dto)
        {
            if (dto == null) return null;
            return new Booking
            {
                BookingId = dto.BookingId,
                DateTime = dto.DateTime,
                Notes = dto.Notes,
                Status = dto.Status,
                CreatedDate = dto.CreatedDate,
                StationId = dto.StationId,
                VehicleId = dto.VehicleId,
                AccountId = dto.AccountId
            };
        }

        public static void UpdateEntity(Booking entity, BookingDTO dto)
        {
            entity.DateTime = dto.DateTime;
            entity.Notes = dto.Notes;
            entity.Status = dto.Status;
            entity.StationId = dto.StationId;
            entity.VehicleId = dto.VehicleId;
            entity.AccountId = dto.AccountId;
        }
    }
}
