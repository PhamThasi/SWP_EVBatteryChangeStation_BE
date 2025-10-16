using EV_BatteryChangeStation_Repository.Entities;
using EV_BatteryChangeStation_Common.DTOs.SubscriptionDTO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EV_BatteryChangeStation_Repository.Mapper
{
    public static class SubscriptionMapper
    {
        public static SubscriptionViewDTO ToDTO(this Subscription sub)
        {
            if (sub == null) return null;

            return new SubscriptionViewDTO
            {
                SubscriptionId = sub.SubscriptionId,
                Name = sub.Name,
                Price = sub.Price,
                ExtraFee = sub.ExtraFee,
                Description = sub.Description,
                DurationPackage = sub.DurationPackage,
                IsActive = sub.IsActive,
                CreateDate = sub.CreateDate,
                UpdateDate = sub.UpdateDate,
                AccountId = sub.AccountId
            };
        }

        public static Subscription ToEntity(this SubscriptionCreateUpdateDTO dto)
        {
            if (dto == null) return null;

            return new Subscription
            {
                Name = dto.Name,
                Price = dto.Price,
                ExtraFee = dto.ExtraFee,
                Description = dto.Description,
                DurationPackage = dto.DurationPackage,
                AccountId = dto.AccountId,
                IsActive = true,
                CreateDate = DateTime.Now
            };
        }

        public static void UpdateEntity(this Subscription sub, SubscriptionCreateUpdateDTO dto)
        {
            if (dto == null || sub == null) return;

            sub.Name = dto.Name;
            sub.Price = dto.Price;
            sub.ExtraFee = dto.ExtraFee;
            sub.Description = dto.Description;
            sub.DurationPackage = dto.DurationPackage;
            sub.AccountId = dto.AccountId;
            sub.UpdateDate = DateTime.Now;
        }

        public static List<SubscriptionViewDTO> ToDTOList(this IEnumerable<Subscription> subs)
        {
            return subs?.Select(s => s.ToDTO()).ToList();
        }
    }
}
