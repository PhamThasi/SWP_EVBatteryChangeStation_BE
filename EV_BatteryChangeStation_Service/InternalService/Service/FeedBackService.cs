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
            try
            {
                var feedbacks = await _unitOfWork.FeedBackRepository.GetAllAsync();
                return feedbacks.Select(f => f.ToFeedBackDTO()).ToList();
            }
            catch (Exception ex)
            {
                // TODO: Log lỗi (nếu có logging)
                throw new Exception("Đã xảy ra lỗi khi lấy danh sách feedbacks.", ex);
            }
        }

        // Lấy feedback theo ID
        public async Task<FeedBackDTO> GetByIdAsync(Guid id)
        {
            try
            {
                var feedback = await _unitOfWork.FeedBackRepository.GetByIdAsync(id);
                if (feedback == null)
                    throw new Exception($"Không tìm thấy feedback với ID = {id}.");

                return feedback.ToFeedBackDTO();
            }
            catch (Exception ex)
            {
                throw new Exception("Đã xảy ra lỗi khi lấy thông tin feedback.", ex);
            }
        }

        // Tạo feedback mới
        public async Task<FeedBackDTO> CreateAsync(CreateFeedBackDTO dto)
        {
            try
            {
                var entity = dto.ToEntity();
                entity.CreateDate = DateTime.Now; // Tự động gán thời gian tạo

                await _unitOfWork.FeedBackRepository.AddAsync(entity);
                await _unitOfWork.CommitAsync();

                return entity.ToFeedBackDTO();
            }
            catch (Exception ex)
            {
                throw new Exception("Đã xảy ra lỗi khi tạo feedback mới.", ex);
            }
        }

        // Cập nhật feedback
        public async Task<FeedBackDTO> UpdateAsync(Guid id, UpdateFeedBackDTO dto)
        {
            try
            {
                var feedback = await _unitOfWork.FeedBackRepository.GetByIdAsync(id);
                if (feedback == null)
                    throw new Exception($"Không tìm thấy feedback với ID = {id}.");

                feedback.Rating = dto.Rating ?? feedback.Rating;
                feedback.Comment = dto.Comment ?? feedback.Comment;
                feedback.AccountId = dto.AccountId ?? feedback.AccountId;
                feedback.BookingId = dto.BookingId ?? feedback.BookingId;

                _unitOfWork.FeedBackRepository.Update(feedback);
                await _unitOfWork.CommitAsync();

                return feedback.ToFeedBackDTO();
            }
            catch (Exception ex)
            {
                throw new Exception("Đã xảy ra lỗi khi cập nhật feedback.", ex);
            }
        }

        // Xóa feedback (xóa cứng)
        public async Task DeleteAsync(Guid id)
        {
            try
            {
                var feedback = await _unitOfWork.FeedBackRepository.GetByIdAsync(id);
                if (feedback == null)
                    throw new Exception($"Không tìm thấy feedback với ID = {id}.");

                _unitOfWork.FeedBackRepository.Delete(feedback);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Đã xảy ra lỗi khi xóa feedback.", ex);
            }
        }
    }
}
