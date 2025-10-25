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

        //  Lấy tất cả trạm
        public async Task<ServiceResult> GetAllAsync()
        {
            try
            {
                var stations = await _unitOfWork.StationRepository.GetAllAsync();
                if (stations == null || !stations.Any())
                    return new ServiceResult(404, "Không có trạm nào trong hệ thống.");

                var data = stations.Select(s => s.ToDTO()).ToList();
                return new ServiceResult(200, "Lấy danh sách trạm thành công.", data);
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Lỗi khi lấy danh sách trạm.", ex.Message);
            }
        }

        // Lấy trạm theo ID
        public async Task<ServiceResult> GetByIdAsync(Guid id)
        {
            try
            {
                var station = await _unitOfWork.StationRepository.GetByIdAsync(id);
                if (station == null)
                    return new ServiceResult(404, $"Không tìm thấy trạm với ID = {id}");

                return new ServiceResult(200, "Lấy thông tin trạm thành công.", station.ToDTO());
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Lỗi khi lấy thông tin trạm.", ex.Message);
            }
        }

        // Tạo mới trạm
        public async Task<ServiceResult> CreateAsync(StationCreateDTO dto)
        {
            try
            {
                if (dto == null)
                    return new ServiceResult(400, "Dữ liệu trạm không hợp lệ.");

                if (string.IsNullOrWhiteSpace(dto.Address))
                    return new ServiceResult(400, "Địa chỉ trạm không được để trống.");
                if (string.IsNullOrWhiteSpace(dto.PhoneNumber))
                    return new ServiceResult(400, "Số điện thoại không được để trống.");

                var entity = dto.ToEntity();

                // Nếu repository bạn không có AddAsync thì dùng Add
                _unitOfWork.StationRepository.Create(entity);
                await _unitOfWork.CommitAsync();

                return new ServiceResult(201, "Tạo trạm mới thành công.", entity.ToDTO());
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Lỗi khi tạo trạm.", ex.Message);
            }
        }

        // Cập nhật trạm
        public async Task<ServiceResult> UpdateAsync(Guid id, StationCreateDTO dto)
        {
            try
            {
                var station = await _unitOfWork.StationRepository.GetByIdAsync(id);
                if (station == null)
                    return new ServiceResult(404, $"Không tìm thấy trạm có ID = {id}");

                // Cập nhật từng trường có giá trị
                if (!string.IsNullOrWhiteSpace(dto.Address))
                    station.Address = dto.Address;
                if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
                    station.PhoneNumber = dto.PhoneNumber;
                if (dto.Status != null)
                    station.Status = dto.Status;
                if (!string.IsNullOrWhiteSpace(dto.AccountName))
                    station.AccountName = dto.AccountName;
                if (dto.BatteryQuantity != null)
                    station.BatteryQuantity = dto.BatteryQuantity;

                _unitOfWork.StationRepository.Update(station);
                await _unitOfWork.CommitAsync();

                return new ServiceResult(200, "Cập nhật trạm thành công.", station.ToDTO());
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Lỗi khi cập nhật trạm.", ex.Message);
            }
        }

        // Xóa mềm (hoặc xóa luôn)
        public async Task<ServiceResult> DeleteAsync(Guid id)
        {
            try
            {
                var station = await _unitOfWork.StationRepository.GetByIdAsync(id);
                if (station == null)
                    return new ServiceResult(404, "Không tìm thấy trạm để xóa.");

                _unitOfWork.StationRepository.Delete(station);
                await _unitOfWork.CommitAsync();

                return new ServiceResult(200, "Đã xóa trạm thành công.");
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Lỗi khi xóa trạm.", ex.Message);
            }
        }

        // Xóa vĩnh viễn
        public async Task<ServiceResult> HardDeleteAsync(Guid id)
        {
            try
            {
                var station = await _unitOfWork.StationRepository.GetByIdAsync(id);
                if (station == null)
                    return new ServiceResult(404, "Không tìm thấy trạm để xóa vĩnh viễn.");

                _unitOfWork.StationRepository.Delete(station);
                await _unitOfWork.CommitAsync();

                return new ServiceResult(200, "Đã xóa trạm vĩnh viễn thành công.");
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Lỗi khi xóa vĩnh viễn trạm.", ex.Message);
            }
        }
    }
}
