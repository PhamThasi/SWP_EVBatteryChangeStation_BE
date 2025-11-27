using EV_BatteryChangeStation_Common.DTOs.PaymentDTO;
using EV_BatteryChangeStation_Service.Base;
using EV_BatteryChangeStation_Service.InternalService.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation.Controllers
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
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentDto create)
        {
            if (create == null)
                return BadRequest("Invalid payment data.");

            var result = await _paymentService.CreatePayment(create);
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
        public async Task<IActionResult> GetPaymentById(Guid paymentId)
        {
            if (paymentId == Guid.Empty)
                return BadRequest("Payment ID is required.");

            var result = await _paymentService.GetPaymentById(paymentId);
            return StatusCode(result.Status, result);
        }

        // =================== GET BY ACCOUNT ===================
        [HttpGet("get-by-account/{accountId}")]
        public async Task<IActionResult> GetPaymentByAccountId(Guid accountId)
        {
            if (accountId == Guid.Empty)
                return BadRequest("Account ID is required.");

            var result = await _paymentService.GetPaymentByAccountId(accountId);
            return StatusCode(result.Status, result);
        }

        // =================== GET BY TRANSACTION ===================
        [HttpGet("get-by-transaction/{transactionId}")]
        public async Task<IActionResult> GetPaymentByTransactionId(Guid transactionId)
        {
            if (transactionId == Guid.Empty)
                return BadRequest("Transaction ID is required.");

            var result = await _paymentService.GetPaymentByTransactionId(transactionId);
            return StatusCode(result.Status, result);
        }

        //// =================== UPDATE ===================
        [HttpPut("update")]
        public async Task<IActionResult> UpdatePayment([FromBody] UpdatePaymentDto updatePaymentDto)
        {
            if (updatePaymentDto.PaymentId == Guid.Empty)
                return BadRequest("Payment ID is required.");

            if (updatePaymentDto == null)
                return BadRequest("Invalid update data.");

            var result = await _paymentService.UpdatePayment(updatePaymentDto);
            return StatusCode(result.Status, result);
        }

        // =================== DELETE (HARD) ===================
        [HttpDelete("delete/{paymentId}")]
        public async Task<IActionResult> DeletePayment(Guid paymentId)
        {
            if (paymentId == Guid.Empty)
                return BadRequest("Payment ID is required.");

            var result = await _paymentService.DeletePayment(paymentId);
            return StatusCode(result.Status, result);
        }

        // =================== DELETE (SOFT) ===================
        [HttpPatch("soft-delete/{paymentId}")]
        public async Task<IActionResult> SoftDeletePayment(Guid paymentId)
        {
            if (paymentId == Guid.Empty)
                return BadRequest("Payment ID is required.");

            var result = await _paymentService.SoftDeletePayment(paymentId);
            return StatusCode(result.Status, result);
        }

        // =================== GET BY GATEWAY ID ===================
        [HttpGet("get-by-gateway/{gatewayId}")]
        public Task<IActionResult> GetByGateWayId(long gatewayid)
        {
            if (gatewayid <= 0)
                return Task.FromResult<IActionResult>(BadRequest("Gateway ID is required."));
            var result = _paymentService.GetByGateWayId(gatewayid);
            return result.ContinueWith(task => (IActionResult)StatusCode(task.Result.Status, task.Result));
        }

        // =================== CHECK SUBSCRIPTION STATUS ===================
        /// <summary>
        /// Check subscription status dựa vào payment để quyết định có cần redirect đến trang thanh toán hay không
        /// Nếu user đã có payment thành công với subscription active và còn hạn thì không cần redirect (needsRedirect = false)
        /// </summary>
        [HttpGet("check-subscription-status")]
        public async Task<IActionResult> CheckSubscriptionStatus([FromQuery] Guid accountId)
        {
            if (accountId == Guid.Empty)
                return BadRequest("Account ID is required.");

            var result = await _paymentService.CheckSubscriptionStatus(accountId);
            return StatusCode(result.Status, result);
        }

    }
}
