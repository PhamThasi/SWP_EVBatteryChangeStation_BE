using EV_BatteryChangeStation_Common.DTOs.SubscriptionDTO;
using EV_BatteryChangeStation_Common.Enum.SubscriptionEnum;
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
            try
            {
                var list = await _unitOfWork.SubscriptionRepository.GetAllAsync();
                var mapped = list.ToDTOList();
                return new ServiceResult(200, "Get all successfully", mapped, SubscriptionErrorCode.None);
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, $"Error while getting all subscriptions: {ex.Message}", null, SubscriptionErrorCode.DatabaseError);
            }
        }

        public async Task<ServiceResult> GetByIdAsync(Guid id)
        {
            try
            {
                var sub = await _unitOfWork.SubscriptionRepository.GetByIdAsync(id);
                if (sub == null)
                    return new ServiceResult(404, "Subscription not found", null, SubscriptionErrorCode.SubscriptionNotFound);

                return new ServiceResult(200, "Success", sub.ToDTO(), SubscriptionErrorCode.None);
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, $"Error while getting subscription by ID: {ex.Message}", null, SubscriptionErrorCode.UnexpectedError);
            }
        }

        public async Task<ServiceResult> CreateAsync(SubscriptionCreateUpdateDTO dto)
        {
            try
            {
                if (dto == null)
                    return new ServiceResult(400, "Invalid subscription data", null, SubscriptionErrorCode.MissingRequiredField);

                var entity = dto.ToEntity();
                entity.CreateDate = DateTime.Now;
                entity.IsActive = true;

                await _unitOfWork.SubscriptionRepository.AddAsync(entity);
                await _unitOfWork.CommitAsync();

                return new ServiceResult(201, "Created successfully", entity.ToDTO(), SubscriptionErrorCode.None);
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, $"Error while creating subscription: {ex.Message}", null, SubscriptionErrorCode.TransactionFailed);
            }
        }

        public async Task<ServiceResult> UpdateAsync(Guid id, SubscriptionCreateUpdateDTO dto)
        {
            try
            {
                var entity = await _unitOfWork.SubscriptionRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ServiceResult(404, "Subscription not found", null, SubscriptionErrorCode.SubscriptionNotFound);

                if (entity.IsActive == false)
                    return new ServiceResult(400, "Cannot update inactive subscription", null, SubscriptionErrorCode.SubscriptionAlreadyExpired);

                entity.UpdateEntity(dto);
                entity.UpdateDate = DateTime.Now;

                _unitOfWork.SubscriptionRepository.Update(entity);
                await _unitOfWork.CommitAsync();

                return new ServiceResult(200, "Updated successfully", entity.ToDTO(), SubscriptionErrorCode.None);
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, $"Error while updating subscription: {ex.Message}", null, SubscriptionErrorCode.DatabaseError);
            }
        }

        public async Task<ServiceResult> SoftDeleteAsync(Guid id)
        {
            try
            {
                var entity = await _unitOfWork.SubscriptionRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ServiceResult(404, "Subscription not found", null, SubscriptionErrorCode.SubscriptionNotFound);

                if (entity.IsActive == false)
                    return new ServiceResult(400, "Subscription already inactive", null, SubscriptionErrorCode.SubscriptionAlreadyCancelled);

                entity.IsActive = false;
                entity.UpdateDate = DateTime.Now;

                _unitOfWork.SubscriptionRepository.Update(entity);
                await _unitOfWork.CommitAsync();

                return new ServiceResult(200, "Soft deleted (IsActive = false)", null, SubscriptionErrorCode.None);
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, $"Error while soft deleting subscription: {ex.Message}", null, SubscriptionErrorCode.TransactionFailed);
            }
        }

        public async Task<ServiceResult> HardDeleteAsync(Guid id)
        {
            try
            {
                var entity = await _unitOfWork.SubscriptionRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ServiceResult(404, "Subscription not found", null, SubscriptionErrorCode.SubscriptionNotFound);

                _unitOfWork.SubscriptionRepository.Delete(entity);
                await _unitOfWork.CommitAsync();

                return new ServiceResult(200, "Hard deleted", null, SubscriptionErrorCode.None);
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, $"Error while hard deleting subscription: {ex.Message}", null, SubscriptionErrorCode.DatabaseError);
            }
        }

        public async Task<ServiceResult> RestoreAsync(Guid id)
        {
            try
            {
                var entity = await _unitOfWork.SubscriptionRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ServiceResult(404, "Subscription not found", null, SubscriptionErrorCode.SubscriptionNotFound);

                if (entity.IsActive == true)
                    return new ServiceResult(400, "Subscription is already active", null, SubscriptionErrorCode.None);

                entity.IsActive = true;
                entity.UpdateDate = DateTime.Now;

                _unitOfWork.SubscriptionRepository.Update(entity);
                await _unitOfWork.CommitAsync();

                return new ServiceResult(200, "Subscription restored successfully", entity.ToDTO(), SubscriptionErrorCode.None);
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, $"Error while restoring subscription: {ex.Message}", null, SubscriptionErrorCode.DatabaseError);
            }
        }

        /// <summary>
        /// Lấy subscription đang active của user (nếu có)
        /// </summary>
        public async Task<ServiceResult> GetActiveByAccountIdAsync(Guid accountId)
        {
            try
            {
                var subscription = await _unitOfWork.SubscriptionRepository.GetActiveByAccountIdAsync(accountId);
                if (subscription == null)
                    return new ServiceResult(404, "No active subscription found for this account", null, SubscriptionErrorCode.SubscriptionNotFound);

                return new ServiceResult(200, "Active subscription retrieved successfully", subscription.ToDTO(), SubscriptionErrorCode.None);
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, $"Error while getting active subscription: {ex.Message}", null, SubscriptionErrorCode.DatabaseError);
            }
        }
    }
}
