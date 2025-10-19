using EV_BatteryChangeStation_Common.DTOs.PaymentDTO;
using EV_BatteryChangeStation_Repository.Entities;
using EV_BatteryChangeStation_Repository.UnitOfWork;
using EV_BatteryChangeStation_Repository.Mapper;
using EV_BatteryChangeStation_Service.Base;
using EV_BatteryChangeStation_Service.InternalService.IService;
using HashidsNet;
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
        private readonly Hashids _hashids;

        public PaymentService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _hashids = new Hashids("EV_BatteryChangeStation", 10);
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

                // Validate subscriptionId (we're NOT using hashids for subscription)
                if (create.SubscriptionId == null || create.SubscriptionId <= 0)
                {
                    return new ServiceResult
                    {
                        Status = Const.FAIL_CREATE_CODE,
                        Message = "Invalid Subscription ID"
                    };
                }

                var subscription = await _unitOfWork.SubscriptionRepository.GetByIdAsync(create.SubscriptionId.Value);
                if (subscription == null)
                {
                    return new ServiceResult
                    {
                        Status = Const.WARNING_NO_DATA_CODE,
                        Message = "Subscription not found"
                    };
                }

                using var scope = await _unitOfWork.BeginTransactionAsync();
                try
                {
                    int? transactionId = null;

                    // If TransactionId provided (encoded), decode it and validate existence.
                    if (!string.IsNullOrEmpty(create.TransactionId))
                    {
                        var decoded = _hashids.Decode(create.TransactionId);
                        if (decoded == null || decoded.Length == 0)
                        {
                            return new ServiceResult
                            {
                                Status = Const.FAIL_CREATE_CODE,
                                Message = "Invalid Transaction ID format"
                            };
                        }

                        transactionId = decoded[0];

                        // IMPORTANT: use the correct repository name for swapping transactions in your UnitOfWork.
                        // If your repo is named differently (e.g. SwappingTransactionRepository), change it here.
                        var swappingTransaction = await _unitOfWork.PaymentRepository.GetByIdAsync(transactionId.Value);
                        if (swappingTransaction == null)
                        {
                            return new ServiceResult
                            {
                                Status = Const.WARNING_NO_DATA_CODE,
                                Message = "Transaction not found"
                            };
                        }
                    }

                    var payment = create.toPayment(create.SubscriptionId, transactionId);
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
        public async Task<IServiceResult> GetPaymentById(string paymentId)
        {
            try
            {
                if (string.IsNullOrEmpty(paymentId))
                {
                    return new ServiceResult
                    {
                        Status = Const.FAIL_READ_CODE,
                        Message = "Payment ID is required"
                    };
                }

                var decoded = _hashids.Decode(paymentId);
                if (decoded == null || decoded.Length == 0)
                {
                    return new ServiceResult
                    {
                        Status = Const.FAIL_READ_CODE,
                        Message = "Invalid Payment ID format"
                    };
                }

                var id = decoded.First();
                var payment = await _unitOfWork.PaymentRepository.GetByIdAsync(id);
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
        public async Task<IServiceResult> GetPaymentByAccountId(string accountId)
        {
            try
            {
                if (string.IsNullOrEmpty(accountId))
                {
                    return new ServiceResult
                    {
                        Status = Const.FAIL_READ_CODE,
                        Message = "Account ID is required"
                    };
                }

                var decoded = _hashids.Decode(accountId);
                if (decoded == null || decoded.Length == 0)
                {
                    return new ServiceResult
                    {
                        Status = Const.FAIL_READ_CODE,
                        Message = "Invalid Account ID format"
                    };
                }

                var accountInt = decoded.First();
                var payments = await _unitOfWork.PaymentRepository.GetPaymentByAccountIdAsync(accountInt);

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

        // =================== GET BY TRANSACTION ===================
        public async Task<IServiceResult> GetPaymentByTransactionId(string transactionId)
        {
            try
            {
                if (string.IsNullOrEmpty(transactionId))
                {
                    return new ServiceResult
                    {
                        Status = Const.FAIL_READ_CODE,
                        Message = "Transaction ID is required"
                    };
                }

                var decoded = _hashids.Decode(transactionId);
                if (decoded == null || decoded.Length == 0)
                {
                    return new ServiceResult
                    {
                        Status = Const.FAIL_READ_CODE,
                        Message = "Invalid Transaction ID format"
                    };
                }

                var txId = decoded.First();
                var payment = await _unitOfWork.PaymentRepository.GetPaymentWithTransactionIdAsync(txId);

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
        public async Task<IServiceResult> DeletePayment(string paymentId)
        {
            try
            {
                if (string.IsNullOrEmpty(paymentId))
                {
                    return new ServiceResult
                    {
                        Status = Const.FAIL_DELETE_CODE,
                        Message = "Payment ID is required"
                    };
                }

                var decoded = _hashids.Decode(paymentId);
                if (decoded == null || decoded.Length == 0)
                {
                    return new ServiceResult
                    {
                        Status = Const.FAIL_DELETE_CODE,
                        Message = "Invalid Payment ID format"
                    };
                }

                var id = decoded.First();
                var payment = await _unitOfWork.PaymentRepository.GetByIdAsync(id);
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
        public async Task<IServiceResult> SoftDeletePayment(string paymentId)
        {
            try
            {
                if (string.IsNullOrEmpty(paymentId))
                {
                    return new ServiceResult
                    {
                        Status = Const.FAIL_DELETE_CODE,
                        Message = "Payment ID is required"
                    };
                }

                var decoded = _hashids.Decode(paymentId);
                if (decoded == null || decoded.Length == 0)
                {
                    return new ServiceResult
                    {
                        Status = Const.FAIL_DELETE_CODE,
                        Message = "Invalid Payment ID format"
                    };
                }

                var id = decoded.First();
                var payment = await _unitOfWork.PaymentRepository.GetByIdAsync(id);
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
    }
}
