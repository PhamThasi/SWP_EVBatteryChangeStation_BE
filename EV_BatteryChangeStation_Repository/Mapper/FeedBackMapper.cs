using EV_BatteryChangeStation_Common.DTOs.FeedBackDTO;
using EV_BatteryChangeStation_Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Repository.Mapper
{
    public static class FeedBackMapper
    {
        // Entity -> DTO
        public static FeedBackDTO ToFeedBackDTO(this Feedback feedback)
        {
            if (feedback == null) return null;

            return new FeedBackDTO
            {
                FeedbackId = feedback.FeedbackId,
                Rating = feedback.Rating,
                Comment = feedback.Comment,
                CreateDate = feedback.CreateDate,
                AccountId = feedback.AccountId,
                BookingId = feedback.BookingId
            };
        }

        // DTO -> Entity (Create)
        public static Feedback ToEntity(this CreateFeedBackDTO dto)
        {
            if (dto == null) return null;

            return new Feedback
            {
                Rating = dto.Rating,
                Comment = dto.Comment,
                AccountId = dto.AccountId,
                BookingId = dto.BookingId,
                CreateDate = DateTime.Now
            };
        }
    }
}
