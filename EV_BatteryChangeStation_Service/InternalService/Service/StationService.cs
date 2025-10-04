using EV_BatteryChangeStation_Common.DTOs.StationDTO;
using EV_BatteryChangeStation_Repository.Entities;
using EV_BatteryChangeStation_Repository.UnitOfWork;
using EV_BatteryChangeStation_Service.InternalService.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Service.InternalService.Service
{
    public class StationService : IStationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<StationDTO>> GetAllAsync()
        {
            var stations = await _unitOfWork.StationRepository.GetAllAsync();
            return stations.Select(s => new StationDTO
            {
                StationId = s.StationId,
                Address = s.Address,
                PhoneNumber = s.PhoneNumber,
                Status = s.Status,
                AccountName = s.AccountName,
                BatteryQuality = s.BatteryQuality
            });
        }

        public async Task<StationDTO> GetByIdAsync(int id)
        {
            var s = await _unitOfWork.StationRepository.GetByIdAsync(id);
            if (s == null) return null;

            return new StationDTO
            {
                StationId = s.StationId,
                Address = s.Address,
                PhoneNumber = s.PhoneNumber,
                Status = s.Status,
                AccountName = s.AccountName,
                BatteryQuality = s.BatteryQuality
            };
        }

        public async Task<StationDTO> CreateAsync(StationDTO dto)
        {
            var station = new Station
            {
                Address = dto.Address,
                PhoneNumber = dto.PhoneNumber,
                Status = dto.Status,
                AccountName = dto.AccountName,
                BatteryQuality = dto.BatteryQuality
            };
            _unitOfWork.StationRepository.Create(station);
            await _unitOfWork.CommitAsync();

            dto.StationId = station.StationId;
            return dto;
        }

        public async Task<bool> UpdateAsync(StationDTO dto)
        {
            var station = await _unitOfWork.StationRepository.GetByIdAsync(dto.StationId);
            if (station == null) return false;

            station.Address = dto.Address;
            station.PhoneNumber = dto.PhoneNumber;
            station.Status = dto.Status;
            station.AccountName = dto.AccountName;
            station.BatteryQuality = dto.BatteryQuality;

            _unitOfWork.StationRepository.Update(station);
            await _unitOfWork.CommitAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var station = await _unitOfWork.StationRepository.GetByIdAsync(id);
            if (station == null) return false;

            _unitOfWork.StationRepository.Delete(station);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
