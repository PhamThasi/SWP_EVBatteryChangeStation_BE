using EV_BatteryChangeStation_Common.DTOs.StationDTO;
using EV_BatteryChangeStation_Repository.Mapper;
using EV_BatteryChangeStation_Repository.UnitOfWork;
using EV_BatteryChangeStation_Service.Base;
using EV_BatteryChangeStation_Service.InternalService.IService;
using System;
using System.Linq;
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

        public async Task<ServiceResult> GetAllAsync()
        {
            try
            {
                var stations = await _unitOfWork.StationRepository.GetAllAsync();
                if (stations == null || !stations.Any())
                    return new ServiceResult(404, "No stations found.");

                var data = stations.ToDTOList();
                return new ServiceResult(200, "Station list retrieved successfully.", data);
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, $"Server error: {ex.Message}");
            }
        }

        public async Task<ServiceResult> GetByIdAsync(int id)
        {
            try
            {
                var station = await _unitOfWork.StationRepository.GetByIdAsync(id);
                if (station == null)
                    return new ServiceResult(404, "Station not found with the given ID.");

                return new ServiceResult(200, "Station retrieved successfully.", station.ToDTO());
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, $"Server error: {ex.Message}");
            }
        }

        public async Task<ServiceResult> CreateAsync(StationDTO dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Address))
                    return new ServiceResult(400, "Address cannot be empty.");

                var station = dto.ToEntity();
                _unitOfWork.StationRepository.Create(station);
                await _unitOfWork.CommitAsync();

                dto.StationId = station.StationId;
                return new ServiceResult(201, "Station created successfully.", dto);
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, $"Error while creating station: {ex.Message}");
            }
        }

        public async Task<ServiceResult> UpdateAsync(StationDTO dto)
        {
            try
            {
                var station = await _unitOfWork.StationRepository.GetByIdAsync(dto.StationId);
                if (station == null)
                    return new ServiceResult(404, "Station not found for update.");

                // ✅ Update only non-empty fields
                if (!string.IsNullOrWhiteSpace(dto.Address))
                    station.Address = dto.Address;

                if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
                    station.PhoneNumber = dto.PhoneNumber;

                if (dto.Status.HasValue)
                    station.Status = dto.Status.Value;

                if (!string.IsNullOrWhiteSpace(dto.AccountName))
                    station.AccountName = dto.AccountName;

                if (!string.IsNullOrWhiteSpace(dto.BatteryQuanity))
                    station.BatteryQuality = dto.BatteryQuality; // ✅ Fix: string không dùng HasValue/Value

                _unitOfWork.StationRepository.Update(station);
                await _unitOfWork.CommitAsync();

                return new ServiceResult(200, "Station updated successfully.");
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, $"Error while updating station: {ex.Message}");
            }
        }

        public async Task<ServiceResult> DeleteAsync(int id)
        {
            try
            {
                var station = await _unitOfWork.StationRepository.GetByIdAsync(id);
                if (station == null)
                    return new ServiceResult(404, "Station not found for deletion.");

                _unitOfWork.StationRepository.Delete(station);
                await _unitOfWork.CommitAsync();

                return new ServiceResult(200, "Station deleted successfully.");
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, $"Error while deleting station: {ex.Message}");
            }
        }
    }
}
