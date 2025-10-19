using EV_BatteryChangeStation_Common.DTOs.PaymentDTO;
using EV_BatteryChangeStation_Service.Base;
using EV_BatteryChangeStation_Service.InternalService.IService;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
        }

        // =================== CREATE ===================
        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentDto createPaymentDto)
        {
            if (createPaymentDto == null)
                return BadRequest("Invalid payment data.");

            var result = await _paymentService.CreatePayment(createPaymentDto);
            return StatusCode(result.Status, result);
        }

        // =================== GET ALL ===================
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllPayments()
        {
            var result = await _paymentService.GetAllPayment();
            return StatusCode(result.Status, result);
        }

        // =================== GET BY ID ===================
        [HttpGet("get-by-id/{paymentId}")]
        public async Task<IActionResult> GetPaymentById(string paymentId)
        {
            if (string.IsNullOrEmpty(paymentId))
                return BadRequest("Payment ID is required.");

            var result = await _paymentService.GetPaymentById(paymentId);
            return StatusCode(result.Status, result);
        }

        // =================== GET BY ACCOUNT ===================
        [HttpGet("get-by-account/{accountId}")]
        public async Task<IActionResult> GetPaymentByAccountId(string accountId)
        {
            if (string.IsNullOrEmpty(accountId))
                return BadRequest("Account ID is required.");

            var result = await _paymentService.GetPaymentByAccountId(accountId);
            return StatusCode(result.Status, result);
        }

        // =================== GET BY TRANSACTION ===================
        [HttpGet("get-by-transaction/{transactionId}")]
        public async Task<IActionResult> GetPaymentByTransactionId(string transactionId)
        {
            if (string.IsNullOrEmpty(transactionId))
                return BadRequest("Transaction ID is required.");

            var result = await _paymentService.GetPaymentByTransactionId(transactionId);
            return StatusCode(result.Status, result);
        }

        //// =================== UPDATE ===================
        //[HttpPut("update/{paymentId}")]
        //public async Task<IActionResult> UpdatePayment(string paymentId, [FromBody] UpdatePaymentDto updatePaymentDto)
        //{
        //    if (string.IsNullOrEmpty(paymentId))
        //        return BadRequest("Payment ID is required.");

        //    if (updatePaymentDto == null)
        //        return BadRequest("Invalid update data.");

        //    var result = await _paymentService.UpdatePayment(paymentId, updatePaymentDto);
        //    return StatusCode(result.Status, result);
        //}

        // =================== DELETE (HARD) ===================
        [HttpDelete("delete/{paymentId}")]
        public async Task<IActionResult> DeletePayment(string paymentId)
        {
            if (string.IsNullOrEmpty(paymentId))
                return BadRequest("Payment ID is required.");

            var result = await _paymentService.DeletePayment(paymentId);
            return StatusCode(result.Status, result);
        }

        // =================== DELETE (SOFT) ===================
        [HttpPatch("soft-delete/{paymentId}")]
        public async Task<IActionResult> SoftDeletePayment(string paymentId)
        {
            if (string.IsNullOrEmpty(paymentId))
                return BadRequest("Payment ID is required.");

            var result = await _paymentService.SoftDeletePayment(paymentId);
            return StatusCode(result.Status, result);
        }
    }
}
