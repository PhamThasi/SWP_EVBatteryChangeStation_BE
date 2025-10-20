using EV_BatteryChangeStation_Common.DTOs.AccountDto;
using EV_BatteryChangeStation_Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Repository.Mapper
{
    public static class AccountMapper
    {
        private static readonly HashidsNet.Hashids _hashids =
           new HashidsNet.Hashids("EV_BatteryChangeStation", 10);
        public static ViewAccountDTOs MapToDTO(this Account account)
        {
            if (account == null) throw new ArgumentNullException(nameof(account), "cannot be null");
            return new ViewAccountDTOs
            {
                RoleId = _hashids.Encode(account.RoleId),
                AccountName = account.AccountName,
                Password = account.Password,
                Address = account.Address,
                Email = account.Email,
                PhoneNumber = account.PhoneNumber,
                DateOfBirth = account.DateOfBirth,
                Status = account.Status,
                FullName = account.FullName,
                Gender = account.Gender,
                CreateDate = account.CreateDate,
                UpdateDate = account.UpdateDate,
            };
        }

        public static Account MapToEntity(this CreateAccountDTO accountDto, int roleId)
        {
            if (accountDto == null) throw new ArgumentNullException(nameof(accountDto));
            return new Account
            {
                FullName = accountDto.FullName,
                RoleId = roleId, // truyền int từ service sau khi decode
                Gender = accountDto.Gender,
                AccountName = accountDto.AccountName,
                Password = accountDto.Password,
                Address = accountDto.Address,
                Email = accountDto.Email,
                PhoneNumber = accountDto.PhoneNumber,
                DateOfBirth = accountDto.DateOfBirth,
                StationId = accountDto.StationId
            };
        }

        public static void MaptoUpdate(this Account account, UpdateAccountDTO updateAccount)
        {
            if (updateAccount == null) throw new ArgumentNullException(nameof(updateAccount), "cannot be null");
            if (updateAccount.RoleId != null)
            {
                account.RoleId = int.Parse(_hashids.Decode(updateAccount.RoleId).First().ToString());
            }
            if (!string.IsNullOrEmpty(updateAccount.AccountName))
            {
                account.AccountName = updateAccount.AccountName;
            }
            if (!string.IsNullOrEmpty(updateAccount.Password))
            {
                account.Password = updateAccount.Password;
            }
            if (!string.IsNullOrEmpty(updateAccount.FullName))
            {
                account.FullName = updateAccount.FullName;
            }
            if (!string.IsNullOrEmpty(updateAccount.Email))
            {
                account.Email = updateAccount.Email;
            }
            if (!string.IsNullOrEmpty(updateAccount.PhoneNumber))
            {
                account.PhoneNumber = updateAccount.PhoneNumber;
            }
            if (!string.IsNullOrEmpty(updateAccount.Address))
            {
                account.Address = updateAccount.Address;
            }
            if (updateAccount.DateOfBirth != null)
            {
                account.DateOfBirth = updateAccount.DateOfBirth;
            }
            if (!string.IsNullOrEmpty(updateAccount.Gender))
            {
                account.Gender = updateAccount.Gender;
            }
        }
        public static List<ViewAccountDTOs> MapToDTO(this List<Account> accounts)
        {
            if (accounts == null) throw new ArgumentNullException(nameof(accounts), "cannot be null");
            return accounts.Select(a => a.MapToDTO()).ToList();
        }
    }
}
