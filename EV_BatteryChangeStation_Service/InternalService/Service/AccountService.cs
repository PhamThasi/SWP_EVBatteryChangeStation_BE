using EV_BatteryChangeStation_Common.DTOs.AccountDto;
using EV_BatteryChangeStation_Common.Enum.ServiceResult;
using EV_BatteryChangeStation_Repository.Entities;
using EV_BatteryChangeStation_Repository.Mapper;
using EV_BatteryChangeStation_Repository.UnitOfWork;
using EV_BatteryChangeStation_Service.Base;
using EV_BatteryChangeStation_Service.InternalService.IService;
using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client;
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
        // Tạo tài khoản mới
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
        //xóa tài khoản
        public async Task<IServiceResult> DeleteAccountAsync(string encodedId)
        {
            try
            {
                if (encodedId == null)
                {
                    return new ServiceResult
                    {
                        Status = Const.ERROR_VALIDATION_CODE,
                        Message = Const.ERROR_INVALID_DATA_MSG
                    };
                }
                var acc = await _unitOfWork.AccountRepository.GetByIdAsync(_hashids.DecodeSingle(encodedId));
                if (acc == null)
                {
                    return new ServiceResult
                    {
                        Status = Const.WARNING_NO_DATA_CODE,
                        Message = Const.WARNING_NO_DATA_MSG
                    };
                }
                await _unitOfWork.AccountRepository.RemoveAsync(acc);
                return new ServiceResult
                {
                    Status = Const.SUCCESS_DELETE_CODE,
                    Message = Const.SUCCESS_DELETE_MSG,
                };
            }
            
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException?.Message ?? ex.Message;
                return new ServiceResult
                {
                    Status = Const.ERROR_EXCEPTION,
                    Message = ex.Message
                };
            }
        }
        //lấy tài khoản dựa vào tên
        public async Task<IServiceResult> GetAccountByNameAsync(string accountName)
        {
            try
            {
                if (string.IsNullOrEmpty(accountName))
                {
                    return new ServiceResult
                    {
                        Status = Const.FAIL_READ_CODE,
                        Message = Const.FAIL_READ_MSG
                    };
                }
                var account = await _unitOfWork.AccountRepository.GetAccountByAccountName(accountName);
                if(account == null)
                {
                    return new ServiceResult
                    {
                        Status = Const.WARNING_NO_DATA_CODE,
                        Message = Const.WARNING_NO_DATA_MSG
                    };
                }
                var accountDto = account.MapToDTO();

                return new ServiceResult
                {
                    Status = Const.SUCCESS_READ_CODE,
                    Message = Const.SUCCESS_READ_MSG,
                    Data = accountDto
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult
                {
                    Status = Const.FAIL_READ_CODE,
                    Message = ex.Message
                };
            }
        }
        //Lấy tất cả tài khoản với id mã hóa
        public async Task<IServiceResult> GetAllAccountWithIdDecodeAsync()
        {
            try
            {
                var account = await _unitOfWork.AccountRepository.GetAllAsync();
                if (account == null || !account.Any())
                {
                    return new ServiceResult
                    {
                        Status = Const.ERROR_VALIDATION_CODE,
                        Message = "No accounts found",
                        Data = null
                    };
                }
                var result = account.Select(a => new ViewAccountDTOs
                {
                    AccountId = _hashids.Encode(a.AccountId),
                    AccountName = a.AccountName,
                    FullName = a.FullName,
                    Password = a.Password,
                    Address = a.Address,
                    PhoneNumber = a.PhoneNumber,
                    Gender = a.Gender,
                    DateOfBirth = a.DateOfBirth
                }).ToList();

                return new ServiceResult
                {
                    Status = Const.SUCCESS_READ_CODE,
                    Message = Const.SUCCESS_READ_MSG,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult
                {
                    Status = Const.ERROR_VALIDATION_CODE,
                    Message = ex.Message,
                };
            }
        }
        // Lấy tất cả tài khoản
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
        //cập nhật tài khoản
        public async Task<IServiceResult> UpdateAccountAsync(UpdateAccountDTO updateAccount)
        {
            try
            {
                if (updateAccount == null || updateAccount.AccountId == null)
                {
                    return new ServiceResult
                    {
                        Status = Const.FAIL_UPDATE_CODE,
                        Message = Const.FAIL_UPDATE_MSG
                    };
                }
                var account = await _unitOfWork.AccountRepository.GetByIdAsync(_hashids.DecodeSingle(updateAccount.AccountId));
                if (account == null)
                {
                    return new ServiceResult
                    {
                        Status = Const.FAIL_UPDATE_CODE,
                        Message = Const.FAIL_UPDATE_MSG
                    };
                }

                account.MaptoUpdate(updateAccount);
                await _unitOfWork.AccountRepository.UpdateAsync(account);
                return new ServiceResult
                {
                    Status = Const.SUCCESS_UPDATE_CODE,
                    Message = Const.SUCCESS_UPDATE_MSG
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult
                {
                    Status = Const.ERROR_EXCEPTION,
                    Message = ex.Message
                };
            }
        }
    }
}
