using EV_BatteryChangeStation_Common.DTOs.StationDTO;
using EV_BatteryChangeStation_Repository.Entities;
using System.Collections.Generic;
using System.Linq;

namespace EV_BatteryChangeStation_Repository.Mapper
{
    public static class StationMapper
    {
        public static StationDTO ToDTO(this Station station)
        {
            if (station == null) return null;

            return new StationDTO
            {
                StationId = station.StationId,
                Address = station.Address,
                PhoneNumber = station.PhoneNumber,
                Status = station.Status,
                StationName = station.StationName,
                BatteryQuantity = station.BatteryQuantity
            };
        }

        public static Station ToEntity(this StationCreateDTO dto)
        {
            if (dto == null) return null;

            return new Station
            {
                Address = dto.Address,
                PhoneNumber = dto.PhoneNumber,
                Status = dto.Status ?? true,
                StationName = dto.StationName,
                BatteryQuantity = dto.BatteryQuantity
            };
        }

        public static List<StationDTO> ToDTOList(this IEnumerable<Station> stations)
        {
            return stations?.Select(s => s.ToDTO()).ToList();
        }
    }
}
