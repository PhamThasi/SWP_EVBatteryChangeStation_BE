using EV_BatteryChangeStation_Common.DTOs.AuthencationDTO;
using EV_BatteryChangeStation_Common.Enum.ServiceResult;
using EV_BatteryChangeStation_Repository.Entities;
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
    public class AuthenService : IAuthenService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IPasswordHasher<Account> _passwordHasher;

        public AuthenService(UnitOfWork unitOfWork, IPasswordHasher<Account> passwordHasher)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        }
        public async Task<IServiceResult> AuthenticationLogin(LoginDTO login)
        {
            var account = await _unitOfWork.AccountRepository.GetByAccountNameOrEmail(login.Keyword.ToLower());
            if (account == null)
            {
                return new ServiceResult
                {
                    Status = Const.WARNING_NO_DATA_CODE,
                    Message = Const.WARNING_NO_DATA_MSG,
                    Errors = new List<string> { "Account not found" }
                };
            }
            if (account.Status == false)
            {
                return new ServiceResult
                {
                    Status = Const.FORBIDDEN_ACCESS_CODE,
                    Message = Const.FORBIDDEN_ACCESS_MSG,
                    Errors = new List<string> { "Your account has been baned. Please contact support for more infomation." }
                };
            }
            var result = _passwordHasher.VerifyHashedPassword(account, account.Password, login.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                return new ServiceResult
                {
                    Status = Const.UNAUTHORIZED_ACCESS_CODE,
                    Message = Const.UNAUTHORIZED_ACCESS_MSG,
                    Errors = new List<string> { "Password is incorrect" }
                };
            }
            var respond = new LoginRespondDTO
            {
                AccountId = account.AccountId,
                AccountName = account.AccountName,
                Email = account.Email,
                RoleName = account.Role?.RoleName
            };
            return new ServiceResult
            {
                Status = Const.SUCCESS_LOGIN_CODE,
                Message = Const.SUCCESS_LOGIN_MSG,
                Data = respond
            };
        }
    }
}
