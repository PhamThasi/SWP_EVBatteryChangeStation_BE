using EV_BatteryChangeStation_Common.DTOs.AccountDto;
using EV_BatteryChangeStation_Common.Enum.ServiceResult;
using EV_BatteryChangeStation_Repository.Entities;
using EV_BatteryChangeStation_Repository.Mapper;
using EV_BatteryChangeStation_Repository.UnitOfWork;
using EV_BatteryChangeStation_Service.Base;
using EV_BatteryChangeStation_Service.InternalService.IService;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Service.InternalService.Service
{
    public class AccountService : IAccountService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly HashidsNet.Hashids _hashids;
        private readonly IPasswordHasher<Account> _passwordHasher;

        public AccountService(UnitOfWork unitOfWork, IPasswordHasher<Account> passwordHasher)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentException(nameof(unitOfWork));
            _hashids = new HashidsNet.Hashids("EV_BatteryChangeStation", 10);
            _passwordHasher = passwordHasher ?? throw new ArgumentException(nameof(passwordHasher));
        }

        public async Task<IServiceResult> CreateAccountAsync(CreateAccountDTO createAccount)
        {
            try
            {
                if (createAccount == null)
                {
                    return new ServiceResult
                    {
                        Status = Const.ERROR_VALIDATION_CODE,
                        Message = "CreateAccountDTO object is null",
                        Data = null
                    };
                }
                // decode roleId từ hashid (nếu roleId truyền lên dạng hash)
                int roleId = _hashids.DecodeSingle(createAccount.RoleId);

                // map sang entity
                var account = createAccount.MapToEntity(roleId);

                // hash password trước khi lưu
                account.Password = _passwordHasher.HashPassword(account, account.Password);
                account.Status = true; // Mặc định tài khoản mới tạo là active
                await _unitOfWork.AccountRepository.CreateAsync(account);
                return new ServiceResult
                {
                    Status = Const.SUCCESS_CREATE_CODE,
                    Message = Const.SUCCESS_CREATE_MSG,
                    Data = account
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task<IServiceResult> DeleteAccountAsync(string encodedId)
        {
            throw new NotImplementedException();
        }

        public Task<IServiceResult> GetAccountByNameAsync(string accountName)
        {
            throw new NotImplementedException();
        }

        public Task<IServiceResult> GetAllAccountByIdDecodeAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IServiceResult> GetAllAccountsAsync()
        {
            try
            {
                var accounts = await _unitOfWork.AccountRepository.GetAllWithRoleAsync();

                if (accounts == null || !accounts.Any())
                {
                    return new ServiceResult
                    {
                        Status = Const.ERROR_VALIDATION_CODE,
                        Message = "No accounts found",
                        Data = null
                    };
                }
                var accountDtos = accounts.Select(a => a.MapToDTO());

                return new ServiceResult
                {
                    Status = Const.SUCCESS_READ_CODE,
                    Message = Const.SUCCESS_READ_MSG,
                    Data = accountDtos
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult
                {
                    Status = Const.ERROR_EXCEPTION,
                    Message = ex.Message,
                    Data = null
                };
            }
        }
        public Task<IServiceResult> UpdateAccountAsync(UpdateAccountDTO updateAccount)
        {
            throw new NotImplementedException();
        }
    }
}
