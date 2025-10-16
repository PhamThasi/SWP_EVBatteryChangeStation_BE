using EV_BatteryChangeStation_Common.DTOs.SupportRequestDTO;
using EV_BatteryChangeStation_Repository.Mapper;
using EV_BatteryChangeStation_Repository.UnitOfWork;
using EV_BatteryChangeStation_Service.Base;
using EV_BatteryChangeStation_Service.InternalService.IService;
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
            var list = await _unitOfWork.SupportRequestRepository.GetAllAsync();
            if (list == null) return new ServiceResult(404, "Support request not found");
            return new ServiceResult(200, "Get all success", list.ToDTOList());
        }

        public async Task<ServiceResult> GetByIdAsync(int id)
        {
            var req = await _unitOfWork.SupportRequestRepository.GetByIdAsync(id);
            if (req == null) return new ServiceResult(404, "Support request not found");
            return new ServiceResult(200, "Get success", req.ToDTO());
        }

        public async Task<ServiceResult> CreateAsync(SupportRequestCreateUpdateDTO dto)
        {
            var entity = dto.ToEntity();
            await _unitOfWork.SupportRequestRepository.CreateAsync(entity);
            await _unitOfWork.CommitAsync();
            return new ServiceResult(201, "Create success", entity.ToDTO());
        }

        public async Task<ServiceResult> UpdateAsync(int id, SupportRequestCreateUpdateDTO dto)
        {
            var req = await _unitOfWork.SupportRequestRepository.GetByIdAsync(id);
            if (req == null) return new ServiceResult(404, "Not found");
            req.UpdateEntity(dto);
            await _unitOfWork.SupportRequestRepository.UpdateAsync(req);
            await _unitOfWork.CommitAsync();
            return new ServiceResult(200, "Update success", req.ToDTO());
        }

        public async Task<ServiceResult> SoftDeleteAsync(int id)
        {
            var req = await _unitOfWork.SupportRequestRepository.GetByIdAsync(id);
            if (req == null) return new ServiceResult(404, "Not found");

            req.Status = false;
            await _unitOfWork.SupportRequestRepository.UpdateAsync(req);
            await _unitOfWork.CommitAsync();
            return new ServiceResult(200, "Soft delete success (status=false)");
        }

        public async Task<ServiceResult> HardDeleteAsync(int id)
        {
            var req = await _unitOfWork.SupportRequestRepository.GetByIdAsync(id);
            if (req == null) return new ServiceResult(404, "Not found");

            await _unitOfWork.SupportRequestRepository.RemoveAsync(req);
            await _unitOfWork.CommitAsync();
            return new ServiceResult(200, "Hard delete success");
        }
    }
}
