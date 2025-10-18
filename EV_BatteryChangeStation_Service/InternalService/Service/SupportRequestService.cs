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
                    return new ServiceResult(404, "No support requests found");

                return new ServiceResult(200, "Get all success", list.ToDTOList());
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, $"Error while getting all support requests: {ex.Message}");
            }
        }

        public async Task<ServiceResult> GetByIdAsync(int id)
        {
            try
            {
                var req = await _unitOfWork.SupportRequestRepository.GetByIdAsync(id);
                if (req == null)
                    return new ServiceResult(404, "Support request not found");

                return new ServiceResult(200, "Get success", req.ToDTO());
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, $"Error while getting support request by ID: {ex.Message}");
            }
        }

        public async Task<ServiceResult> CreateAsync(SupportRequestCreateUpdateDTO dto)
        {
            try
            {
                var entity = dto.ToEntity();
                await _unitOfWork.SupportRequestRepository.CreateAsync(entity);
                await _unitOfWork.CommitAsync();

                return new ServiceResult(201, "Create success", entity.ToDTO());
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, $"Error while creating support request: {ex.Message}");
            }
        }

        public async Task<ServiceResult> UpdateAsync(int id, SupportRequestCreateUpdateDTO dto)
        {
            try
            {
                var req = await _unitOfWork.SupportRequestRepository.GetByIdAsync(id);
                if (req == null)
                    return new ServiceResult(404, "Support request not found");

                req.UpdateEntity(dto);
                await _unitOfWork.SupportRequestRepository.UpdateAsync(req);
                await _unitOfWork.CommitAsync();

                return new ServiceResult(200, "Update success", req.ToDTO());
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, $"Error while updating support request: {ex.Message}");
            }
        }

        public async Task<ServiceResult> SoftDeleteAsync(int id)
        {
            try
            {
                var req = await _unitOfWork.SupportRequestRepository.GetByIdAsync(id);
                if (req == null)
                    return new ServiceResult(404, "Support request not found");

                req.Status = false;
                await _unitOfWork.SupportRequestRepository.UpdateAsync(req);
                await _unitOfWork.CommitAsync();

                return new ServiceResult(200, "Soft delete success (status=false)");
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, $"Error while soft deleting support request: {ex.Message}");
            }
        }

        public async Task<ServiceResult> HardDeleteAsync(int id)
        {
            try
            {
                var req = await _unitOfWork.SupportRequestRepository.GetByIdAsync(id);
                if (req == null)
                    return new ServiceResult(404, "Support request not found");

                await _unitOfWork.SupportRequestRepository.RemoveAsync(req);
                await _unitOfWork.CommitAsync();

                return new ServiceResult(200, "Hard delete success");
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, $"Error while hard deleting support request: {ex.Message}");
            }
        }
    }
}
