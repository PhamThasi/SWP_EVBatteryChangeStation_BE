using EV_BatteryChangeStation_Common.DTOs.FeedBackDTO;
using EV_BatteryChangeStation_Repository.UnitOfWork;
using EV_BatteryChangeStation_Service.InternalService.IService;
using EV_BatteryChangeStation_Repository.Mapper;
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

        // Lấy tất cả feedbacks
        public async Task<List<FeedBackDTO>> GetAllAsync()
        {
            var feedbacks = await _unitOfWork.FeedBackRepository.GetAllAsync();
            return feedbacks.Select(f => f.ToFeedBackDTO()).ToList();
        }

        // Lấy feedback theo ID
        public async Task<FeedBackDTO> GetByIdAsync(int id)
        {
            var feedback = await _unitOfWork.FeedBackRepository.GetByIdAsync(id);
            return feedback?.ToFeedBackDTO();
        }

        // Tạo feedback mới và trả về DTO
        public async Task<FeedBackDTO> CreateAsync(CreateFeedBackDTO dto)
        {
            var entity = dto.ToEntity();
            entity.CreateDate = DateTime.Now; // tự động gán thời gian tạo

            await _unitOfWork.FeedBackRepository.AddAsync(entity);
            await _unitOfWork.CommitAsync();

            return entity.ToFeedBackDTO();
        }

        // Cập nhật feedback theo ID và trả về DTO
        public async Task<FeedBackDTO> UpdateAsync(int id, UpdateFeedBackDTO dto)
        {
            var feedback = await _unitOfWork.FeedBackRepository.GetByIdAsync(id);
            if (feedback == null)
                throw new Exception("Feedback not found");

            feedback.Rating = dto.Rating ?? feedback.Rating;
            feedback.Comment = dto.Comment ?? feedback.Comment;
            feedback.AccountId = dto.AccountId ?? feedback.AccountId;
            feedback.BookingId = dto.BookingId ?? feedback.BookingId;

            _unitOfWork.FeedBackRepository.Update(feedback);
            await _unitOfWork.CommitAsync();

            return feedback.ToFeedBackDTO();
        }

        // Xóa feedback (xóa cứng)
        public async Task DeleteAsync(int id)
        {
            var feedback = await _unitOfWork.FeedBackRepository.GetByIdAsync(id);
            if (feedback == null)
                throw new Exception("Feedback not found");

            _unitOfWork.FeedBackRepository.Delete(feedback);
            await _unitOfWork.CommitAsync();
        }
    }
}
