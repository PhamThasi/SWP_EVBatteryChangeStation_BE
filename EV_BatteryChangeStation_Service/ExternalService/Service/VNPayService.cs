using EV_BatteryChangeStation_Common.Enum.ServiceResult;
using EV_BatteryChangeStation_Repository.Mapper;
using EV_BatteryChangeStation_Repository.UnitOfWork;
using EV_BatteryChangeStation_Service.Base;
using EV_BatteryChangeStation_Service.ExternalService.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VNPAY.NET;
using VNPAY.NET.Enums;
using VNPAY.NET.Models;

namespace EV_BatteryChangeStation_Service.ExternalService.Service
{
    public class VNPayService : IVNPayService
    {
        private readonly IVnpay _vnPay;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;

        public VNPayService(IVnpay vnPay, IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _vnPay = vnPay;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _vnPay.Initialize(
                _configuration["Vnpay:TmnCode"],
                _configuration["Vnpay:HashSecret"],
                _configuration["Vnpay:BaseUrl"],
                _configuration["Vnpay:ReturnUrl"]
            );
        }

        public async Task<IServiceResult> CreatePaymentURL(Guid payment, string ipAddress)
        {
            try
            {
                using var scop = await _unitOfWork.BeginTransactionAsync();
                try
                {
                    var payment1 = await _unitOfWork.PaymentRepository.GetByIdAsync(payment);
                    if (payment1 == null)
                    {
                        return new ServiceResult
                        {
                            Status = Const.FAIL_READ_CODE,
                            Message = "Payment transaction not found",
                            Data = null
                        };
                    }
                    var request = new PaymentRequest
                    {
                        PaymentId = payment1.PaymentGateId.Value,
                        Money = Convert.ToDouble(payment1.Price),
                        Description = "payment for transaction ",
                        IpAddress = ipAddress,
                        BankCode = BankCode.ANY,
                        CreatedDate = DateTime.Now.AddMinutes(20),
                        Currency = Currency.VND,
                        Language = DisplayLanguage.Vietnamese,
                    };

                    var paymentUrl = _vnPay.GetPaymentUrl(request);
                    await scop.CommitAsync();

                    return new ServiceResult(Const.SUCCESS_CREATE_CODE, "Create payment URL success", paymentUrl);
                }
                catch (Exception ex)
                {
                    await scop.RollbackAsync();
                    return new ServiceResult
                    {
                        Status = Const.ERROR_EXCEPTION,
                        Message = ex.Message,
                        Data = null
                    };
                }
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

        public async Task<IServiceResult> ValidateRespond(IQueryCollection queryParams)
        {
            try
            {
                if (queryParams == null || !queryParams.Any())
                {
                    return new ServiceResult(Const.FAIL_READ_CODE, "Invalid query parameter");
                }
                var paymentResult = _vnPay.GetPaymentResult(queryParams);

                if (paymentResult == null)
                {
                    return new ServiceResult(Const.FAIL_READ_CODE, " Payment result failed");
                }

                var gateway = paymentResult.PaymentId;
                var transaction = await _unitOfWork.PaymentRepository.GetByGatewayIdAsync(gateway);
                if (transaction == null)
                {
                    return new ServiceResult(Const.FAIL_READ_CODE, "Transaction not found");
                }

                transaction.UpdateToPaymentVNPay(paymentResult);
                await _unitOfWork.PaymentRepository.UpdateAsync(transaction);

                return new ServiceResult(Const.SUCCESS_PAYMENT_CODE, Const.SUCCESS_PAYMENT_MSG, paymentResult);
            }
            catch (Exception ex)
            {
                string errorMessage = ex.InnerException?.Message ?? ex.Message;
                Console.WriteLine($"VNPayService.ValidateRespond error: {errorMessage}");
                return new ServiceResult(Const.ERROR_EXCEPTION, errorMessage);
            }
        }
    }
}
