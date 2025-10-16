using EV_BatteryChangeStation_Common.DTOs.SubscriptionDTO;
using EV_BatteryChangeStation_Repository.Mapper;
using EV_BatteryChangeStation_Repository.UnitOfWork;
using EV_BatteryChangeStation_Service.Base;
using EV_BatteryChangeStation_Service.InternalService.IService;
using System;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Service.InternalService.Service
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubscriptionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult> GetAllAsync()
        {
            var list = await _unitOfWork.SubscriptionRepository.GetAllAsync();
            var mapped = list.ToDTOList();
            return new ServiceResult(200, "Get all successfully", mapped);
        }

        public async Task<ServiceResult> GetByIdAsync(int id)
        {
            var sub = await _unitOfWork.SubscriptionRepository.GetByIdAsync(id);
            if (sub == null)
                return new ServiceResult(404, "Subscription not found");

            return new ServiceResult(200, "Success", sub.ToDTO());
        }

        public async Task<ServiceResult> CreateAsync(SubscriptionCreateUpdateDTO dto)
        {
            var entity = dto.ToEntity();

            await _unitOfWork.SubscriptionRepository.AddAsync(entity);
            await _unitOfWork.CommitAsync();

            return new ServiceResult(201, "Created successfully", entity.ToDTO());
        }

        public async Task<ServiceResult> UpdateAsync(int id, SubscriptionCreateUpdateDTO dto)
        {
            var entity = await _unitOfWork.SubscriptionRepository.GetByIdAsync(id);
            if (entity == null)
                return new ServiceResult(404, "Subscription not found");

            entity.UpdateEntity(dto);
            _unitOfWork.SubscriptionRepository.Update(entity);
            await _unitOfWork.CommitAsync();

            return new ServiceResult(200, "Updated successfully", entity.ToDTO());
        }

        public async Task<ServiceResult> SoftDeleteAsync(int id)
        {
            var entity = await _unitOfWork.SubscriptionRepository.GetByIdAsync(id);
            if (entity == null)
                return new ServiceResult(404, "Subscription not found");

            entity.IsActive = false;
            entity.UpdateDate = DateTime.Now;

            _unitOfWork.SubscriptionRepository.Update(entity);
            await _unitOfWork.CommitAsync(); 

            return new ServiceResult(200, "Soft deleted (IsActive = false)");
        }

        public async Task<ServiceResult> HardDeleteAsync(int id)
        {
            var entity = await _unitOfWork.SubscriptionRepository.GetByIdAsync(id);
            if (entity == null)
                return new ServiceResult(404, "Subscription not found");

            _unitOfWork.SubscriptionRepository.Delete(entity);
            await _unitOfWork.CommitAsync(); 

            return new ServiceResult(200, "Hard deleted");
        }
    }
}
