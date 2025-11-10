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
                    return new ServiceResult(404, "No support requests found.");

                return new ServiceResult(200, "Successfully retrieved support request list.", list.ToDTOList());
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Error while retrieving support request list.", ex.Message);
            }
        }

        public async Task<ServiceResult> GetByIdAsync(Guid id)
        {
            try
            {
                var req = await _unitOfWork.SupportRequestRepository.GetByIdAsync(id);
                if (req == null)
                    return new ServiceResult(404, "Support request not found.");

                return new ServiceResult(200, "Successfully retrieved support request.", req.ToDTO());
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Error while retrieving support request.", ex.Message);
            }
        }

        public async Task<ServiceResult> CreateAsync(SupportRequestCreateDTO dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.IssueType))
                    return new ServiceResult(400, "Issue type (IssueType) cannot be empty.");

                var account = await _unitOfWork.AccountRepository.GetByIdAsync(dto.AccountId);
                if (account == null)
                    return new ServiceResult(404, "The account submitting the request does not exist.");

                var entity = dto.ToEntity();
                entity.CreateDate = DateTime.Now;
                entity.Status = true;

                await _unitOfWork.SupportRequestRepository.CreateAsync(entity);
                await _unitOfWork.CommitAsync();

                return new ServiceResult(201, "Support request created successfully.", entity.ToDTO());
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Error while creating support request.", ex.Message);
            }
        }

        public async Task<ServiceResult> UpdateAsync(Guid id, SupportRequestUpdateDTO dto)
        {
            try
            {
                var req = await _unitOfWork.SupportRequestRepository.GetByIdAsync(id);
                if (req == null)
                    return new ServiceResult(404, "Support request not found for update.");

                req.UpdateEntity(dto);
                req.ResponseDate = DateTime.Now;

                await _unitOfWork.SupportRequestRepository.UpdateAsync(req);
                await _unitOfWork.CommitAsync();

                return new ServiceResult(200, "Support request updated successfully.", req.ToDTO());
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Error while updating support request.", ex.Message);
            }
        }

        public async Task<ServiceResult> SoftDeleteAsync(Guid id)
        {
            try
            {
                var req = await _unitOfWork.SupportRequestRepository.GetByIdAsync(id);
                if (req == null)
                    return new ServiceResult(404, "Support request not found for deletion.");

                req.Status = false;
                await _unitOfWork.SupportRequestRepository.UpdateAsync(req);
                await _unitOfWork.CommitAsync();

                return new ServiceResult(200, "Support request has been deactivated (Soft Delete).");
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Error while deleting support request.", ex.Message);
            }
        }

        public async Task<ServiceResult> HardDeleteAsync(Guid id)
        {
            try
            {
                var req = await _unitOfWork.SupportRequestRepository.GetByIdAsync(id);
                if (req == null)
                    return new ServiceResult(404, "Support request not found for permanent deletion.");

                await _unitOfWork.SupportRequestRepository.RemoveAsync(req);
                await _unitOfWork.CommitAsync();

                return new ServiceResult(200, "Support request permanently deleted.");
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Error while permanently deleting support request.", ex.Message);
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
                    return new ServiceResult(404, "No support requests found for this account.");

                return new ServiceResult(200, "Successfully retrieved support requests by AccountId.", filtered.ToDTOList());
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Error while retrieving support requests by AccountId.", ex.Message);
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
                    return new ServiceResult(404, "No support requests found assigned to this staff.");

                return new ServiceResult(200, "Successfully retrieved support requests by StaffId.", filtered.ToDTOList());
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Error while retrieving support requests by StaffId.", ex.Message);
            }
        }
    }
}
