using EV_BatteryChangeStation_Common.DTOs.FeedBackDTO;
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
    public class FeedBackService : IFeedBackService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FeedBackService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult> GetAllAsync()
        {
            try
            {
                var feedbacks = await _unitOfWork.FeedBackRepository.GetAllAsync();
                if (feedbacks == null || !feedbacks.Any())
                    return new ServiceResult(404, "No feedbacks found.");

                var data = feedbacks.Select(f => f.ToFeedBackDTO()).ToList();
                return new ServiceResult(200, "Successfully retrieved feedback list.", data);
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Error while retrieving feedbacks.", ex.Message);
            }
        }

        public async Task<ServiceResult> GetByIdAsync(Guid id)
        {
            try
            {
                var feedback = await _unitOfWork.FeedBackRepository.GetByIdAsync(id);
                if (feedback == null)
                    return new ServiceResult(404, $"Feedback with ID = {id} not found.");

                return new ServiceResult(200, "Feedback retrieved successfully.", feedback.ToFeedBackDTO());
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Error while retrieving feedback.", ex.Message);
            }
        }

        public async Task<ServiceResult> CreateAsync(CreateFeedBackDTO dto)
        {
            try
            {
                if (dto.Rating == null || dto.Rating < 1 || dto.Rating > 5)
                    return new ServiceResult(400, "Rating must be between 1 and 5.");

                if (string.IsNullOrWhiteSpace(dto.Comment))
                    dto.Comment = "User did not leave a comment.";

                var account = await _unitOfWork.AccountRepository.GetByIdAsync(dto.AccountId);
                if (account == null)
                    return new ServiceResult(404, "This account does not exist.");

                var booking = await _unitOfWork.BookingRepository.GetByIdAsync(dto.BookingId);
                if (booking == null)
                    return new ServiceResult(404, "The corresponding booking for feedback does not exist.");

                var entity = dto.ToEntity();
                entity.CreateDate = DateTime.Now;

                await _unitOfWork.FeedBackRepository.AddAsync(entity);
                await _unitOfWork.CommitAsync();

                return new ServiceResult(201, "Feedback created successfully.", entity.ToFeedBackDTO());
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Error while creating feedback.", ex.Message);
            }
        }

        public async Task<ServiceResult> UpdateAsync(Guid id, UpdateFeedBackDTO dto)
        {
            try
            {
                var feedback = await _unitOfWork.FeedBackRepository.GetByIdAsync(id);
                if (feedback == null)
                    return new ServiceResult(404, $"Feedback with ID = {id} not found.");

                if (dto.Rating.HasValue && (dto.Rating < 1 || dto.Rating > 5))
                    return new ServiceResult(400, "Rating must be between 1 and 5.");

                feedback.Rating = dto.Rating ?? feedback.Rating;
                feedback.Comment = dto.Comment ?? feedback.Comment;
                feedback.AccountId = dto.AccountId ?? feedback.AccountId;
                feedback.BookingId = dto.BookingId ?? feedback.BookingId;

                _unitOfWork.FeedBackRepository.Update(feedback);
                await _unitOfWork.CommitAsync();

                return new ServiceResult(200, "Feedback updated successfully.", feedback.ToFeedBackDTO());
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Error while updating feedback.", ex.Message);
            }
        }

        public async Task<ServiceResult> DeleteAsync(Guid id)
        {
            try
            {
                var feedback = await _unitOfWork.FeedBackRepository.GetByIdAsync(id);
                if (feedback == null)
                    return new ServiceResult(404, $"Feedback with ID = {id} not found.");

                _unitOfWork.FeedBackRepository.Delete(feedback);
                await _unitOfWork.CommitAsync();

                return new ServiceResult(200, "Feedback deleted successfully.");
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Error while deleting feedback.", ex.Message);
            }
        }
        public async Task<ServiceResult> GetByAccountIdAsync(Guid accountId)
        {
            try
            {
                var feedbacks = await _unitOfWork.FeedBackRepository.GetByAccountIdAsync(accountId);
                if (feedbacks == null || !feedbacks.Any())
                    return new ServiceResult(404, $"No feedbacks found for Account ID = {accountId}.");

                var data = feedbacks.Select(f => f.ToFeedBackDTO()).ToList();
                return new ServiceResult(200, "Successfully retrieved feedbacks by account.", data);
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Error while retrieving feedbacks by account.", ex.Message);
            }
        }

    }
}
