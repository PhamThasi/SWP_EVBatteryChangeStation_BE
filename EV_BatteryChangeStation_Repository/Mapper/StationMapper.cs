using EV_BatteryChangeStation_Common.DTOs.StationDTO;
using EV_BatteryChangeStation_Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Repository.Mapper
{
    public static class StationMapper
    {
        // Entity -> DTO
        public static StationDTO ToDTO(this Station station)
        {
            if (station == null) return null;

            return new StationDTO
            {
                StationId = station.StationId,
                Address = station.Address,
                PhoneNumber = station.PhoneNumber,
                Status = station.Status,
                AccountName = station.AccountName,
                BatteryQuality = station.BatteryQuality
            };
        }

        // DTO -> Entity
        public static Station ToEntity(this StationDTO dto)
        {
            if (dto == null) return null;

            return new Station
            {
                StationId = dto.StationId,
                Address = dto.Address,
                PhoneNumber = dto.PhoneNumber,
                Status = dto.Status ?? true, // default true nếu null
                AccountName = dto.AccountName,
                BatteryQuality = dto.BatteryQuality
            };
        }

        // List<Entity> -> List<DTO>
        public static List<StationDTO> ToDTOList(this IEnumerable<Station> stations)
        {
            return stations?.Select(s => s.ToDTO()).ToList();
        }
    }
}
