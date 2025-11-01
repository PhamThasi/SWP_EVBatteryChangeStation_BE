using EV_BatteryChangeStation_Repository.Entities;
using EV_BatteryChangeStation_Common.DTOs.SupportRequestDTO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EV_BatteryChangeStation_Repository.Mapper
{
    public static class SupportRequestMapper
    {
        public static SupportRequestViewDTO ToDTO(this SupportRequest req)
        {
            if (req == null) return null;

            return new SupportRequestViewDTO
            {
                RequestId = req.RequestId,
                IssueType = req.IssueType,
                Description = req.Description,
                CreateDate = req.CreateDate,
                Status = req.Status,
                AccountId = req.AccountId,
                StaffId = req.StaffId,
                ResponseText = req.ResponseText,
                ResponseDate = req.ResponseDate
            };
        }

        public static SupportRequest ToEntity(this SupportRequestCreateDTO dto)
        {
            if (dto == null) return null;

            return new SupportRequest
            {
                IssueType = dto.IssueType,
                Description = dto.Description,
                AccountId = dto.AccountId,
                ResponseText = dto.ResponseText,
                CreateDate = DateTime.Now,
                ResponseDate = dto.ResponseText != null ? DateTime.Now : null,
                Status = true
            };
        }

        public static void UpdateEntity(this SupportRequest req, SupportRequestUpdateDTO dto)
        {
            if (dto == null || req == null) return;

            req.IssueType = dto.IssueType;
            req.Description = dto.Description;
            req.StaffId = dto.StaffId;

            if (!string.IsNullOrWhiteSpace(dto.ResponseText))
            {
                req.ResponseText = dto.ResponseText;
                req.ResponseDate = DateTime.Now;
            }
        }


        public static List<SupportRequestViewDTO> ToDTOList(this IEnumerable<SupportRequest> list)
        {
            return list?.Select(r => r.ToDTO()).ToList();
        }
    }
}
