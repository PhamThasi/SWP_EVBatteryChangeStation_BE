using EV_BatteryChangeStation_Common.DTOs.StationDTO;
using EV_BatteryChangeStation_Repository.Entities;
using EV_BatteryChangeStation_Repository.Mapper;
using EV_BatteryChangeStation_Repository.UnitOfWork;
using EV_BatteryChangeStation_Service.Base;
using EV_BatteryChangeStation_Service.InternalService.IService;
using System;
using System.Collections.Generic;
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

        // Get all stations
        public async Task<ServiceResult> GetAllAsync()
        {
            try
            {
                var stations = await _unitOfWork.StationRepository.GetAllAsync();
                if (stations == null || !stations.Any())
                    return new ServiceResult(404, "No stations found in the system.");

                var data = stations.Select(s => s.ToDTO()).ToList();
                return new ServiceResult(200, "Successfully retrieved station list.", data);
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Error while retrieving station list.", ex.Message);
            }
        }

        // Get station by ID
        public async Task<ServiceResult> GetByIdAsync(Guid id)
        {
            try
            {
                var station = await _unitOfWork.StationRepository.GetByIdAsync(id);
                if (station == null)
                    return new ServiceResult(404, $"Station with ID = {id} not found.");

                return new ServiceResult(200, "Successfully retrieved station information.", station.ToDTO());
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Error while retrieving station information.", ex.Message);
            }
        }

        public async Task<ServiceResult> SearchByNameAsync(string keyword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                    return new ServiceResult(400, "Keyword cannot be empty.");

                var stations = await _unitOfWork.StationRepository.SearchByNameAsync(keyword);
                if (stations == null || !stations.Any())
                    return new ServiceResult(404, $"No stations found for keyword '{keyword}'.");

                return new ServiceResult(200, "Stations retrieved successfully.", stations.ToDTOList());
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Error while searching stations.", ex.Message);
            }
        }

        public async Task<ServiceResult> GetByNameAsync(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                    return new ServiceResult(400, "Station name cannot be empty.");

                var station = await _unitOfWork.StationRepository.GetByNameAsync(name);
                if (station == null)
                    return new ServiceResult(404, $"Station '{name}' not found.");

                var dto = new StationDTO
                {
                    StationId = station.StationId,
                    StationName = station.StationName
                };
                return new ServiceResult(200, "Found station.", dto);
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Error while looking up station.", ex.Message);
            }
        }

        // Create a new station
        public async Task<ServiceResult> CreateAsync(StationCreateDTO dto)
        {
            try
            {
                if (dto == null)
                    return new ServiceResult(400, "Invalid station data.");

                if (string.IsNullOrWhiteSpace(dto.Address))
                    return new ServiceResult(400, "Station address cannot be empty.");
                if (string.IsNullOrWhiteSpace(dto.PhoneNumber))
                    return new ServiceResult(400, "Phone number cannot be empty.");

                var entity = dto.ToEntity();

                
                _unitOfWork.StationRepository.Create(entity);
                await _unitOfWork.CommitAsync();

                return new ServiceResult(201, "Station created successfully.", entity.ToDTO());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CreateAsync ERROR] {ex}");
                return new ServiceResult(500, "Error while creating station.", ex.Message);
            }
        }

        // Update station
        public async Task<ServiceResult> UpdateAsync(Guid id, StationCreateDTO dto)
        {
            try
            {
                var station = await _unitOfWork.StationRepository.GetByIdAsync(id);
                if (station == null)
                    return new ServiceResult(404, $"Station with ID = {id} not found.");

                // Update each field if it has a value
                if (!string.IsNullOrWhiteSpace(dto.Address))
                    station.Address = dto.Address;
                if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
                    station.PhoneNumber = dto.PhoneNumber;
                if (dto.Status != null)
                    station.Status = dto.Status;
                if (!string.IsNullOrWhiteSpace(dto.StationName))
                    station.StationName = dto.StationName;
                if (dto.BatteryQuantity != null)
                    station.BatteryQuantity = dto.BatteryQuantity;

                _unitOfWork.StationRepository.Update(station);
                await _unitOfWork.CommitAsync();

                return new ServiceResult(200, "Station updated successfully.", station.ToDTO());
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Error while updating station.", ex.Message);
            }
        }

        // Soft delete (or normal delete)
        public async Task<ServiceResult> DeleteAsync(Guid id)
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
                return new ServiceResult(500, "Error while deleting station.", ex.Message);
            }
        }

        // Hard delete
        public async Task<ServiceResult> HardDeleteAsync(Guid id)
        {
            try
            {
                var station = await _unitOfWork.StationRepository.GetByIdAsync(id);
                if (station == null)
                    return new ServiceResult(404, "Station not found for permanent deletion.");

                _unitOfWork.StationRepository.Delete(station);
                await _unitOfWork.CommitAsync();

                return new ServiceResult(200, "Station permanently deleted successfully.");
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Error while permanently deleting station.", ex.Message);
            }
        }
    }
}
