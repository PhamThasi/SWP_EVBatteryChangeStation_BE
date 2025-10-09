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
                var activeStations = stations.ToList();

                if (!activeStations.Any())
                    return new ServiceResult(404, "No active stations found.");

                var data = activeStations.ToDTOList();
                return new ServiceResult(200, "Active station list retrieved successfully.", data);
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

        public async Task<ServiceResult> CreateAsync(StationCreateDTO dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Address))
                    return new ServiceResult(400, "Address cannot be empty.");

                var station = dto.ToEntity();
                _unitOfWork.StationRepository.Create(station);
                await _unitOfWork.CommitAsync();

                // Lấy lại entity sau khi thêm (đảm bảo có StationId)
                var createdStation = await _unitOfWork.StationRepository.GetByIdAsync(station.StationId);
                var stationDto = createdStation.ToDTO();

                return new ServiceResult(201, "Station created successfully.", stationDto);
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, $"Error while creating station: {ex.Message}");
            }
        }

        public async Task<ServiceResult> UpdateAsync(int id, StationCreateDTO dto)
        {
            try
            {
                var station = await _unitOfWork.StationRepository.GetByIdAsync(id);
                if (station == null)
                    return new ServiceResult(404, "Station not found for update.");

                // Cập nhật các trường nếu có giá trị mới
                if (!string.IsNullOrWhiteSpace(dto.Address))
                    station.Address = dto.Address;

                if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
                    station.PhoneNumber = dto.PhoneNumber;

                if (dto.Status.HasValue)
                    station.Status = dto.Status.Value;

                if (!string.IsNullOrWhiteSpace(dto.AccountName))
                    station.AccountName = dto.AccountName;

                if (dto.BatteryQuantity.HasValue)
                    station.BatteryQuantity = dto.BatteryQuantity.Value;

                _unitOfWork.StationRepository.Update(station);
                await _unitOfWork.CommitAsync();

                var updatedStation = await _unitOfWork.StationRepository.GetByIdAsync(id);
                var stationDto = updatedStation.ToDTO();

                return new ServiceResult(200, "Station updated successfully.", stationDto);
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

                // Soft delete: chỉ cập nhật trạng thái
                station.Status = false;

                _unitOfWork.StationRepository.Update(station);
                await _unitOfWork.CommitAsync();

                return new ServiceResult(200, "Station deactivated (soft deleted) successfully.");
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, $"Error while deactivating station: {ex.Message}");
            }
        }

        public async Task<ServiceResult> HardDeleteAsync(int id)
        {
            try
            {
                var station = await _unitOfWork.StationRepository.GetByIdAsync(id);
                if (station == null)
                    return new ServiceResult(404, "Station not found for hard delete.");

                _unitOfWork.StationRepository.Delete(station);
                await _unitOfWork.CommitAsync();

                return new ServiceResult(200, "Station deleted permanently.");
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, $"Error while deleting station permanently: {ex.Message}");
            }
        }
    }
}
