using EV_BatteryChangeStation_Common.DTOs.BookingDTO;
using EV_BatteryChangeStation_Common.Enum.BookingEnum;
using EV_BatteryChangeStation_Repository.Entities;
using System;

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
                IsApproved = entity.IsApproved, // dùng enum helper
                CreatedDate = entity.CreatedDate,
                StationId = entity.StationId,
                VehicleId = entity.VehicleId,
                AccountId = entity.AccountId,
                BatteryId = entity.BatteryId
            };
        }

        public static Booking ToEntity(BookingCreateDTO dto)
        {
            if (dto == null) return null;

            var entity = new Booking
            {
                DateTime = dto.DateTime,
                Notes = dto.Notes,
                CreatedDate = dto.CreatedDate,
                StationId = dto.StationId,
                VehicleId = dto.VehicleId,
                AccountId = dto.AccountId,
                BatteryId = dto.BatteryId,
                IsApproved= dto.IsApproved
            };

            // Gán enum thông qua helper ApprovalStatus
            //entity.ApprovalStatus = dto.IsApproved; // dto.IsApproved phải là BookingApprovalStatus

            return entity;
        }

        public static void UpdateEntity(Booking entity, BookingCreateDTO dto)
        {
            entity.DateTime = dto.DateTime;
            entity.Notes = dto.Notes;
            entity.StationId = dto.StationId;
            entity.VehicleId = dto.VehicleId;
            entity.AccountId = dto.AccountId;
            entity.BatteryId = dto.BatteryId;
            entity.IsApproved = dto.IsApproved;
        }


    }
}
