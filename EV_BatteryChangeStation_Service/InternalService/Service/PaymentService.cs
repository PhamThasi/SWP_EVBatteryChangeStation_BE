using EV_BatteryChangeStation_Common.DTOs.PaymentDTO;
using EV_BatteryChangeStation_Repository.Entities;
using EV_BatteryChangeStation_Repository.UnitOfWork;
using EV_BatteryChangeStation_Repository.Mapper;
using EV_BatteryChangeStation_Service.Base;
using EV_BatteryChangeStation_Service.InternalService.IService;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using EV_BatteryChangeStation_Common.Enum.ServiceResult;
using Microsoft.EntityFrameworkCore;
using EV_BatteryChangeStation_Common.Enum.PaymentEnum;

namespace EV_BatteryChangeStation_Service.InternalService.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        // =================== CREATE ===================
        public async Task<IServiceResult> CreatePayment(CreatePaymentDto create)
        {
            try
            {
                if (create == null)
                {
                    return new ServiceResult
                    {
                        Status = Const.FAIL_CREATE_CODE,
                        Message = Const.FAIL_CREATE_MSG
                    };
                }

                // Validate: Phải có SubscriptionId HOẶC TransactionId (không thể cả hai đều null)
                if (!create.SubscriptionId.HasValue && !create.TransactionId.HasValue)
                {
                    return new ServiceResult
                    {
                        Status = Const.FAIL_CREATE_CODE,
                        Message = "Either SubscriptionId or TransactionId is required"
                    };
                }

                // Nếu có SubscriptionId → Phải có AccountId
                if (create.SubscriptionId.HasValue && !create.AccountId.HasValue)
                {
                    return new ServiceResult
                    {
                        Status = Const.FAIL_CREATE_CODE,
                        Message = "AccountId is required when purchasing subscription"
                    };
                }

                // Nếu có TransactionId → Kiểm tra transaction tồn tại
                if (create.TransactionId.HasValue)
                {
                    var transaction = await _unitOfWork.SwappingTransactionRepository.GetByIdAsync(create.TransactionId.Value);
                    if (transaction == null)
                    {
                        return new ServiceResult
                        {
                            Status = Const.WARNING_NO_DATA_CODE,
                            Message = "Transaction not found"
                        };
                    }
                }

                // Nếu có SubscriptionId → Kiểm tra subscription tồn tại
                Subscription subscription = null;
                if (create.SubscriptionId.HasValue)
                {
                    subscription = await _unitOfWork.SubscriptionRepository.GetByIdAsync(create.SubscriptionId.Value);
                    if (subscription == null)
                    {
                        return new ServiceResult
                        {
                            Status = Const.WARNING_NO_DATA_CODE,
                            Message = "Subscription not found"
                        };
                    }
                }

                using var scope = await _unitOfWork.BeginTransactionAsync();
                try
                {
                    var payment = create.toPayment();
                    payment.Status = PaymentEnum.Pending.ToString();

                    await _unitOfWork.PaymentRepository.CreateAsync(payment);
                    await scope.CommitAsync();

                    return new ServiceResult
                    {
                        Status = Const.SUCCESS_CREATE_CODE,
                        Message = Const.SUCCESS_CREATE_MSG,
                        Data = payment.PaymentRespondDto()
                    };
                }
                catch (Exception ex)
                {
                    await scope.RollbackAsync();
                    throw new Exception("Error while creating payment", ex);
                }
            }
            catch (DbUpdateException dbEx)
            {
                return new ServiceResult
                {
                    Status = Const.ERROR_EXCEPTION,
                    Message = dbEx.InnerException?.Message ?? dbEx.Message
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult
                {
                    Status = Const.ERROR_EXCEPTION,
                    Message = ex.InnerException?.Message ?? ex.Message
                };
            }
        }

        // =================== GET ALL ===================
        public async Task<IServiceResult> GetAllPayment()
        {
            try
            {
                var payments = await _unitOfWork.PaymentRepository.GetAllPaymentDetailAsync();

                if (payments == null || payments.Count == 0)
                {
                    return new ServiceResult
                    {
                        Status = Const.WARNING_NO_DATA_CODE,
                        Message = Const.WARNING_NO_DATA_MSG
                    };
                }

                var dtoList = payments.Select(p => p.PaymentRespondDto()).ToList();

                return new ServiceResult
                {
                    Status = Const.SUCCESS_READ_CODE,
                    Message = Const.SUCCESS_READ_MSG,
                    Data = dtoList
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult
                {
                    Status = Const.ERROR_EXCEPTION,
                    Message = ex.InnerException?.Message ?? ex.Message
                };
            }
        }

        // =================== GET BY ID ===================
        public async Task<IServiceResult> GetPaymentById(Guid paymentId)
        {
            try
            {
                if (paymentId == Guid.Empty)
                {
                    return new ServiceResult
                    {
                        Status = Const.FAIL_READ_CODE,
                        Message = "Payment ID is required"
                    };
                }

                var payment = await _unitOfWork.PaymentRepository.GetByIdAsync(paymentId);
                if (payment == null)
                {
                    return new ServiceResult
                    {
                        Status = Const.WARNING_NO_DATA_CODE,
                        Message = "Payment not found"
                    };
                }

                return new ServiceResult
                {
                    Status = Const.SUCCESS_READ_CODE,
                    Message = Const.SUCCESS_READ_MSG,
                    Data = payment.PaymentRespondDto()
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult
                {
                    Status = Const.ERROR_EXCEPTION,
                    Message = ex.InnerException?.Message ?? ex.Message
                };
            }
        }

        // =================== GET BY ACCOUNT ===================
        public async Task<IServiceResult> GetPaymentByAccountId(Guid accountId)
        {
            try
            {
                if (accountId == Guid.Empty)
                {
                    return new ServiceResult
                    {
                        Status = Const.FAIL_READ_CODE,
                        Message = "Account ID is required"
                    };
                }

                var payments = await _unitOfWork.PaymentRepository.GetPaymentHistoryByAccountIdAsync(accountId);

                if (payments == null)
                {
                    return new ServiceResult
                    {
                        Status = Const.WARNING_NO_DATA_CODE,
                        Message = Const.WARNING_NO_DATA_MSG
                    };
                }

                var dtoList = payments.Select(p => p.PaymentRespondDto()).ToList();

                return new ServiceResult
                {
                    Status = Const.SUCCESS_READ_CODE,
                    Message = Const.SUCCESS_READ_MSG,
                    Data = dtoList
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult
                {
                    Status = Const.ERROR_EXCEPTION,
                    Message = ex.InnerException?.Message ?? ex.Message
                };
            }
        }

        // =================== GET BY TRANSACTION ===================
        public async Task<IServiceResult> GetPaymentByTransactionId(Guid transactionId)
        {
            try
            {
                if (transactionId == Guid.Empty)
                {
                    return new ServiceResult
                    {
                        Status = Const.FAIL_READ_CODE,
                        Message = "Transaction ID is required"
                    };
                }

                var payment = await _unitOfWork.PaymentRepository.GetPaymentWithTransactionIdAsync(transactionId);

                if (payment == null)
                {
                    return new ServiceResult
                    {
                        Status = Const.WARNING_NO_DATA_CODE,
                        Message = "No payment found for this transaction"
                    };
                }

                return new ServiceResult
                {
                    Status = Const.SUCCESS_READ_CODE,
                    Message = Const.SUCCESS_READ_MSG,
                    Data = payment.PaymentRespondDto()
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult
                {
                    Status = Const.ERROR_EXCEPTION,
                    Message = ex.InnerException?.Message ?? ex.Message
                };
            }
        }

        // =================== UPDATE ===================
        public async Task<IServiceResult> UpdatePayment(UpdatePaymentDto update)
        {
            try
            {
                if (update.PaymentId == Guid.Empty)
                {
                    return new ServiceResult
                    {
                        Status = Const.FAIL_UPDATE_CODE,
                        Message = "Payment ID is required"
                    };
                }
                var payment = await _unitOfWork.PaymentRepository.GetByIdAsync(update.PaymentId);

                if(payment == null)
                {
                    return new ServiceResult
                    {
                        Status = Const.WARNING_NO_DATA_CODE,
                        Message = "Payment not found"
                    };
                }

                bool canConfirm = await _unitOfWork.PaymentRepository.CheckPaymentOwnerAsync(update.AccountId, payment);
                if (!canConfirm)
                {
                    return new ServiceResult
                    {
                        Status = Const.FAIL_UPDATE_CODE,
                        Message = "Account không có quyền xác nhận thanh toán này"
                    };
                }


                payment.UpdateToPayment(update);
                await _unitOfWork.PaymentRepository.UpdateAsync(payment);
                return new ServiceResult
                {
                    Status = Const.SUCCESS_UPDATE_CODE,
                    Message = Const.SUCCESS_UPDATE_MSG,
                    Data = payment.PaymentRespondDto()
                };

            }
            catch (Exception ex)
            {
                return new ServiceResult
                {
                    Status = Const.ERROR_EXCEPTION,
                    Message = ex.InnerException?.Message ?? ex.Message
                };
            }
        }

        // =================== DELETE(HARD) ===================
        public async Task<IServiceResult> DeletePayment(Guid paymentId)
        {
            try
            {
                if (paymentId == Guid.Empty)
                {
                    return new ServiceResult
                    {
                        Status = Const.FAIL_DELETE_CODE,
                        Message = "Payment ID is required"
                    };
                }

                var payment = await _unitOfWork.PaymentRepository.GetByIdAsync(paymentId);
                if (payment == null)
                {
                    return new ServiceResult
                    {
                        Status = Const.WARNING_NO_DATA_CODE,
                        Message = "Payment not found"
                    };
                }

                await _unitOfWork.PaymentRepository.RemoveAsync(payment);

                return new ServiceResult
                {
                    Status = Const.SUCCESS_DELETE_CODE,
                    Message = Const.SUCCESS_DELETE_MSG
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult
                {
                    Status = Const.ERROR_EXCEPTION,
                    Message = ex.InnerException?.Message ?? ex.Message
                };
            }
        }

        // =================== SOFT DELETE ===================
        public async Task<IServiceResult> SoftDeletePayment(Guid paymentId)
        {
            try
            {
                if (paymentId == Guid.Empty)
                {
                    return new ServiceResult
                    {
                        Status = Const.FAIL_DELETE_CODE,
                        Message = "Payment ID is required"
                    };
                }

                var payment = await _unitOfWork.PaymentRepository.GetByIdAsync(paymentId);
                if (payment == null)
                {
                    return new ServiceResult
                    {
                        Status = Const.WARNING_NO_DATA_CODE,
                        Message = "Payment not found"
                    };
                }

                payment.Status = PaymentEnum.Canceled.ToString();
                await _unitOfWork.PaymentRepository.UpdateAsync(payment);

                return new ServiceResult
                {
                    Status = Const.SUCCESS_DELETE_CODE,
                    Message = "Payment soft deleted successfully",
                    Data = payment.PaymentRespondDto()
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult
                {
                    Status = Const.ERROR_EXCEPTION,
                    Message = ex.InnerException?.Message ?? ex.Message
                };
            }
        }
        //// =================== VALIDATE PAYMENT ===================
        //public async Task<IServiceResult> ValidatePayment(ValidatePaymentDto validate)
        //{
        //    try
        //    {
        //        using var scope = await _unitOfWork.BeginTransactionAsync();
        //        try
        //        {
        //            var payment = await _unitOfWork.PaymentRepository.GetByIdAsync(validate.PaymentId);
        //            if (payment == null)
        //            {
        //                return new ServiceResult
        //                {
        //                    Status = Const.WARNING_NO_DATA_CODE,
        //                    Message = "Payment not found"
        //                };
        //            }
        //            if (payment.SubscriptionId != validate.SubcriptionId || payment.TransactionId != validate.TransactionId)
        //            {
        //                return new ServiceResult
        //                {
        //                    Status = Const.FAIL_VALIDATE_CODE,
        //                    Message = "Payment validation failed"
        //                };
        //            }
        //            return new ServiceResult
        //            {
        //                Status = Const.SUCCESS_READ_CODE,
        //                Message = "Payment validated successfully",
        //                Data = payment.PaymentRespondDto()
        //            };
        //        }
        //        catch (Exception ex)
        //        {
        //            await scope.RollbackAsync();
        //            throw new Exception("Error while validating payment", ex);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ServiceResult
        //        {
        //            Status = Const.ERROR_EXCEPTION,
        //            Message = ex.InnerException?.Message ?? ex.Message
        //        };
        //    }
        //}
        // =================== GET BY GATEWAY ID ===================
        public async Task<IServiceResult> GetByGateWayId(long gateway)
        {
            try
            {
                var payment = await _unitOfWork.PaymentRepository.GetByGatewayIdAsync(gateway);
                if (payment == null)
                {
                    return new ServiceResult
                    {
                        Status = Const.WARNING_NO_DATA_CODE,
                        Message = "Payment not found"
                    };
                }
                return new ServiceResult
                {
                    Status = Const.SUCCESS_READ_CODE,
                    Message = Const.SUCCESS_READ_MSG,
                    Data = payment.PaymentRespondDto()
                };
            }
            catch(Exception ex)
            {
                return new ServiceResult
                {
                    Status = Const.ERROR_EXCEPTION,
                    Message = ex.InnerException?.Message ?? ex.Message
                };
            }
        }

        // =================== CHECK SUBSCRIPTION STATUS ===================
        /// <summary>
        /// Check subscription status dựa vào payment để quyết định có cần redirect đến trang thanh toán hay không
        /// Nếu user đã có payment thành công với subscription active và còn hạn thì không cần redirect
        /// </summary>
        public async Task<IServiceResult> CheckSubscriptionStatus(Guid accountId)
        {
            try
            {
                if (accountId == Guid.Empty)
                {
                    return new ServiceResult
                    {
                        Status = Const.FAIL_READ_CODE,
                        Message = "Account ID is required"
                    };
                }

                var payment = await _unitOfWork.PaymentRepository.GetActiveSubscriptionPaymentByAccountIdAsync(accountId);
                
                var statusCheck = new SubscriptionStatusCheckDto
                {
                    HasActiveSubscription = payment != null,
                    NeedsRedirect = payment == null, // Nếu không có payment thành công với subscription active thì cần redirect
                    Payment = payment?.PaymentRespondDto()
                };

                var message = payment != null 
                    ? "User has active subscription from successful payment, no redirect needed" 
                    : "User does not have active subscription, redirect to payment page needed";

                return new ServiceResult
                {
                    Status = Const.SUCCESS_READ_CODE,
                    Message = message,
                    Data = statusCheck
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult
                {
                    Status = Const.ERROR_EXCEPTION,
                    Message = ex.InnerException?.Message ?? ex.Message
                };
            }
        }
    }
}
    
