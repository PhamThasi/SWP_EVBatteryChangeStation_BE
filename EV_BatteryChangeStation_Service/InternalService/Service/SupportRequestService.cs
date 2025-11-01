using EV_BatteryChangeStation_Common.DTOs.SupportRequestDTO;
using EV_BatteryChangeStation_Repository.Mapper;
using EV_BatteryChangeStation_Repository.UnitOfWork;
using EV_BatteryChangeStation_Service.Base;
using EV_BatteryChangeStation_Service.InternalService.IService;
using System;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Service.InternalService.Service
{
    public class SupportRequestService : ISupportRequestService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SupportRequestService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult> GetAllAsync()
        {
            try
            {
                var list = await _unitOfWork.SupportRequestRepository.GetAllAsync();
                if (list == null || list.Count == 0)
                    return new ServiceResult(404, "Không có yêu cầu hỗ trợ nào.");

                return new ServiceResult(200, "Lấy danh sách thành công.", list.ToDTOList());
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Lỗi khi lấy danh sách yêu cầu hỗ trợ.", ex.Message);
            }
        }

        public async Task<ServiceResult> GetByIdAsync(Guid id)
        {
            try
            {
                var req = await _unitOfWork.SupportRequestRepository.GetByIdAsync(id);
                if (req == null)
                    return new ServiceResult(404, "Không tìm thấy yêu cầu hỗ trợ.");

                return new ServiceResult(200, "Lấy yêu cầu thành công.", req.ToDTO());
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Lỗi khi lấy yêu cầu hỗ trợ.", ex.Message);
            }
        }

        public async Task<ServiceResult> CreateAsync(SupportRequestCreateDTO dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.IssueType))
                    return new ServiceResult(400, "Loại sự cố (IssueType) không được để trống.");

                var account = await _unitOfWork.AccountRepository.GetByIdAsync(dto.AccountId);
                if (account == null)
                    return new ServiceResult(404, "Không tồn tại tài khoản gửi yêu cầu.");

                var entity = dto.ToEntity();
                entity.CreateDate = DateTime.Now;
                entity.Status = true;

                await _unitOfWork.SupportRequestRepository.CreateAsync(entity);
                await _unitOfWork.CommitAsync();

                return new ServiceResult(201, "Tạo yêu cầu hỗ trợ thành công.", entity.ToDTO());
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Lỗi khi tạo yêu cầu hỗ trợ.", ex.Message);
            }
        }

        public async Task<ServiceResult> UpdateAsync(Guid id, SupportRequestUpdateDTO dto)
        {
            try
            {
                var req = await _unitOfWork.SupportRequestRepository.GetByIdAsync(id);
                if (req == null)
                    return new ServiceResult(404, "Không tìm thấy yêu cầu hỗ trợ để cập nhật.");

                req.UpdateEntity(dto);
                req.ResponseDate = DateTime.Now;

                await _unitOfWork.SupportRequestRepository.UpdateAsync(req);
                await _unitOfWork.CommitAsync();

                return new ServiceResult(200, "Cập nhật yêu cầu hỗ trợ thành công.", req.ToDTO());
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Lỗi khi cập nhật yêu cầu hỗ trợ.", ex.Message);
            }
        }


        public async Task<ServiceResult> SoftDeleteAsync(Guid id)
        {
            try
            {
                var req = await _unitOfWork.SupportRequestRepository.GetByIdAsync(id);
                if (req == null)
                    return new ServiceResult(404, "Không tìm thấy yêu cầu hỗ trợ để xóa.");

                req.Status = false;
                await _unitOfWork.SupportRequestRepository.UpdateAsync(req);
                await _unitOfWork.CommitAsync();

                return new ServiceResult(200, "Đã vô hiệu hóa yêu cầu hỗ trợ (Soft Delete).");
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Lỗi khi xóa yêu cầu hỗ trợ.", ex.Message);
            }
        }

        public async Task<ServiceResult> HardDeleteAsync(Guid id)
        {
            try
            {
                var req = await _unitOfWork.SupportRequestRepository.GetByIdAsync(id);
                if (req == null)
                    return new ServiceResult(404, "Không tìm thấy yêu cầu hỗ trợ để xóa vĩnh viễn.");

                await _unitOfWork.SupportRequestRepository.RemoveAsync(req);
                await _unitOfWork.CommitAsync();

                return new ServiceResult(200, "Đã xóa vĩnh viễn yêu cầu hỗ trợ.");
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Lỗi khi xóa vĩnh viễn yêu cầu hỗ trợ.", ex.Message);
            }
        }
        public async Task<ServiceResult> GetByAccountIdAsync(Guid accountId)
        {
            try
            {
                var list = await _unitOfWork.SupportRequestRepository
                    .GetAllAsync();

                var filtered = list.Where(r => r.AccountId == accountId).ToList();

                if (filtered == null || filtered.Count == 0)
                    return new ServiceResult(404, "Không có yêu cầu hỗ trợ nào cho tài khoản này.");

                return new ServiceResult(200, "Lấy danh sách yêu cầu theo AccountId thành công.", filtered.ToDTOList());
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Lỗi khi lấy danh sách yêu cầu theo AccountId.", ex.Message);
            }
        }

        public async Task<ServiceResult> GetByStaffIdAsync(Guid staffId)
        {
            try
            {
                var list = await _unitOfWork.SupportRequestRepository
                    .GetAllAsync();

                var filtered = list.Where(r => r.StaffId == staffId).ToList();

                if (filtered == null || filtered.Count == 0)
                    return new ServiceResult(404, "Không có yêu cầu hỗ trợ nào do nhân viên này phụ trách.");

                return new ServiceResult(200, "Lấy danh sách yêu cầu theo StaffId thành công.", filtered.ToDTOList());
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Lỗi khi lấy danh sách yêu cầu theo StaffId.", ex.Message);
            }
        }

    }
}
