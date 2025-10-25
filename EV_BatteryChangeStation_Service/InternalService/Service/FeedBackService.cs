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
                    return new ServiceResult(404, "Không có feedback nào được tìm thấy.");

                var data = feedbacks.Select(f => f.ToFeedBackDTO()).ToList();
                return new ServiceResult(200, "Lấy danh sách feedback thành công.", data);
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Lỗi khi lấy danh sách feedbacks.", ex.Message);
            }
        }

        public async Task<ServiceResult> GetByIdAsync(Guid id)
        {
            try
            {
                var feedback = await _unitOfWork.FeedBackRepository.GetByIdAsync(id);
                if (feedback == null)
                    return new ServiceResult(404, $"Không tìm thấy feedback có ID = {id}");

                return new ServiceResult(200, "Lấy feedback thành công.", feedback.ToFeedBackDTO());
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Lỗi khi lấy feedback.", ex.Message);
            }
        }

        public async Task<ServiceResult> CreateAsync(CreateFeedBackDTO dto)
        {
            try
            {
                if (dto.Rating == null || dto.Rating < 1 || dto.Rating > 5)
                    return new ServiceResult(400, "Điểm đánh giá (Rating) phải từ 1 đến 5.");

                if (string.IsNullOrWhiteSpace(dto.Comment))
                    dto.Comment = "Người dùng không để lại bình luận.";

                var account = await _unitOfWork.AccountRepository.GetByIdAsync(dto.AccountId);
                if (account == null)
                    return new ServiceResult(404, "Không tồn tại tài khoản này.");

                var booking = await _unitOfWork.BookingRepository.GetByIdAsync(dto.BookingId);
                if (booking == null)
                    return new ServiceResult(404, "Không tồn tại booking tương ứng để feedback.");

                var entity = dto.ToEntity();
                entity.CreateDate = DateTime.Now;

                await _unitOfWork.FeedBackRepository.AddAsync(entity);
                await _unitOfWork.CommitAsync();

                return new ServiceResult(201, "Tạo feedback thành công.", entity.ToFeedBackDTO());
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Lỗi khi tạo feedback.", ex.Message);
            }
        }

        public async Task<ServiceResult> UpdateAsync(Guid id, UpdateFeedBackDTO dto)
        {
            try
            {
                var feedback = await _unitOfWork.FeedBackRepository.GetByIdAsync(id);
                if (feedback == null)
                    return new ServiceResult(404, $"Không tìm thấy feedback với ID = {id}");

                if (dto.Rating.HasValue && (dto.Rating < 1 || dto.Rating > 5))
                    return new ServiceResult(400, "Điểm đánh giá (Rating) phải từ 1 đến 5.");

                feedback.Rating = dto.Rating ?? feedback.Rating;
                feedback.Comment = dto.Comment ?? feedback.Comment;
                feedback.AccountId = dto.AccountId ?? feedback.AccountId;
                feedback.BookingId = dto.BookingId ?? feedback.BookingId;

                _unitOfWork.FeedBackRepository.Update(feedback);
                await _unitOfWork.CommitAsync();

                return new ServiceResult(200, "Cập nhật feedback thành công.", feedback.ToFeedBackDTO());
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Lỗi khi cập nhật feedback.", ex.Message);
            }
        }

        public async Task<ServiceResult> DeleteAsync(Guid id)
        {
            try
            {
                var feedback = await _unitOfWork.FeedBackRepository.GetByIdAsync(id);
                if (feedback == null)
                    return new ServiceResult(404, $"Không tìm thấy feedback với ID = {id}");

                _unitOfWork.FeedBackRepository.Delete(feedback);
                await _unitOfWork.CommitAsync();

                return new ServiceResult(200, "Xóa feedback thành công.");
            }
            catch (Exception ex)
            {
                return new ServiceResult(500, "Lỗi khi xóa feedback.", ex.Message);
            }
        }
    }
}
