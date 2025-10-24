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

namespace EV_BatteryChangeStation_Service.InternalService.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly UnitOfWork _unitOfWork;

        public PaymentService(UnitOfWork unitOfWork)
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

                // ✅ Validate SubscriptionId và TransactionId
                if (create.SubscriptionId == Guid.Empty || create.TransactionId == Guid.Empty)
                {
                    return new ServiceResult
                    {
                        Status = Const.FAIL_CREATE_CODE,
                        Message = "Invalid Subscription ID or Transaction ID"
                    };
                }

                // ✅ Kiểm tra subscription tồn tại
                var subscription = await _unitOfWork.SubscriptionRepository.GetByIdAsync(create.SubscriptionId);
                if (subscription == null)
                {
                    return new ServiceResult
                    {
                        Status = Const.WARNING_NO_DATA_CODE,
                        Message = "Subscription not found"
                    };
                }

                // ✅ Kiểm tra transaction tồn tại
                var transaction = await _unitOfWork.SwappingTransactionRepository.GetByIdAsync(create.TransactionId);
                if (transaction == null)
                {
                    return new ServiceResult
                    {
                        Status = Const.WARNING_NO_DATA_CODE,
                        Message = "Transaction not found"
                    };
                }

                using var scope = await _unitOfWork.BeginTransactionAsync();
                try
                {
                    // ✅ Tạo payment mới
                    var payment = create.toPayment(); // không truyền tham số — mapper tự xử lý
                    payment.SubscriptionId = create.SubscriptionId;
                    payment.TransactionId = create.TransactionId;

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

                if (accountId == Guid.Empty)
                {
                    return new ServiceResult
                    {
                        Status = Const.FAIL_READ_CODE,
                        Message = "Invalid Account ID format"
                    };
                }

                var payments = await _unitOfWork.PaymentRepository.GetPaymentByAccountIdAsync(accountId);

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

                if (transactionId == Guid.Empty)
                {
                    return new ServiceResult
                    {
                        Status = Const.FAIL_READ_CODE,
                        Message = "Invalid Transaction ID format"
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

        //// =================== UPDATE ===================
        //public async Task<IServiceResult> UpdatePayment(string paymentId, UpdatePaymentDto update)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(paymentId))
        //        {
        //            return new ServiceResult
        //            {
        //                Status = Const.FAIL_UPDATE_CODE,
        //                Message = "Payment ID is required"
        //            };
        //        }

        //        // ✅ Decode PaymentId (vẫn dùng hash)
        //        var decodedPayment = _hashids.Decode(paymentId);
        //        if (decodedPayment == null || decodedPayment.Length == 0)
        //        {
        //            return new ServiceResult
        //            {
        //                Status = Const.FAIL_UPDATE_CODE,
        //                Message = "Invalid Payment ID format"
        //            };
        //        }
        //        var id = decodedPayment.First();

        //        // ✅ Lấy payment từ DB
        //        var payment = await _unitOfWork.PaymentRepository.GetByIdAsync(id);
        //        if (payment == null)
        //        {
        //            return new ServiceResult
        //            {
        //                Status = Const.WARNING_NO_DATA_CODE,
        //                Message = "Payment not found"
        //            };
        //        }

        //        // ✅ Decode TransactionId nếu có
        //        if (!string.IsNullOrEmpty(update.TransactionId))
        //        {
        //            var decodedTransaction = _hashids.Decode(update.TransactionId);
        //            if (decodedTransaction == null || decodedTransaction.Length == 0)
        //            {
        //                return new ServiceResult
        //                {
        //                    Status = Const.FAIL_UPDATE_CODE,
        //                    Message = "Invalid Transaction ID format"
        //                };
        //            }

        //            update.TransactionId = decodedTransaction.First().ToString();
        //        }

        //        // ✅ Cập nhật dữ liệu
        //        payment.UpdateToPayment(update);
        //        await _unitOfWork.PaymentRepository.UpdateAsync(payment);

        //        return new ServiceResult
        //        {
        //            Status = Const.SUCCESS_UPDATE_CODE,
        //            Message = Const.SUCCESS_UPDATE_MSG,
        //            Data = payment.PaymentRespondDto()
        //        };
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

        // =================== DELETE (HARD) ===================
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

                if (paymentId == Guid.Empty)
                {
                    return new ServiceResult
                    {
                        Status = Const.FAIL_DELETE_CODE,
                        Message = "Invalid Payment ID format"
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

                if (paymentId == Guid.Empty)
                {
                    return new ServiceResult
                    {
                        Status = Const.FAIL_DELETE_CODE,
                        Message = "Invalid Payment ID format"
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

                payment.Status = false;
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
    }
}
    
