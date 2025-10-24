using EV_BatteryChangeStation_Common.Enum.ServiceResult;
using EV_BatteryChangeStation_Service.Base;
using EV_BatteryChangeStation_Service.ExternalService.IService;
using Microsoft.AspNetCore.Mvc;
using VNPAY.NET.Utilities;

namespace EV_BatteryChangeStation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VNPayController : ControllerBase
    {
        private readonly IVNPayService _vnPayService;

        public VNPayController(IVNPayService vnPayService)
        {
            _vnPayService = vnPayService;
        }

        [HttpPost("create-payment")]
        public async Task<IActionResult> CreatePayment(Guid paymentId)
        {
            string ipAddress = NetworkHelper.GetIpAddress(HttpContext);
            var result = await _vnPayService.CreatePaymentURL(paymentId, ipAddress);
            if (result.Status == Const.SUCCESS_CREATE_CODE)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("validate-respond")]
        public async Task<IActionResult> ValidateRespond()
        {
            try
            {
                var result = await _vnPayService.ValidateRespond(HttpContext.Request.Query);
                if (result.Status == Const.SUCCESS_PAYMENT_CODE)
                    return Ok(result);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ServiceResult(Const.FAIL_READ_CODE, ex.Message));
            }
        }

        [HttpGet("vnpay-return")]
        public async Task<IActionResult> VNPayReturn()
        {
            var queryParams = HttpContext.Request.Query;
            var result = await _vnPayService.ValidateRespond(queryParams);

            // Kiểm tra nếu có lỗi null để tránh crash
            if (result == null)
            {
                return BadRequest("VNPay result is null");
            }

            return Ok(result);
        }
    }
}
