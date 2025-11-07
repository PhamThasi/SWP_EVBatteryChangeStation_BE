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
            // Thêm try-catch ở đây để an toàn
            try
            {
                string ipAddress = NetworkHelper.GetIpAddress(HttpContext);
                var result = await _vnPayService.CreatePaymentURL(paymentId, ipAddress);
                if (result.Status == Const.SUCCESS_CREATE_CODE)
                    return Ok(result);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ServiceResult(Const.ERROR_EXCEPTION, $"Controller Error: {ex.Message}"));
            }
        }

        [HttpGet("validate-respond")]
        public async Task<IActionResult> ValidateRespond()
        {
            // Thêm try-catch ở đây để an toàn
            try
            {
                var result = await _vnPayService.ValidateRespond(HttpContext.Request.Query);
                if (result.Status == Const.SUCCESS_PAYMENT_CODE)
                    return Ok(result);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ServiceResult(Const.FAIL_READ_CODE, $"Controller Error: {ex.Message}"));
            }
        }

        // ========= ĐÂY LÀ HÀM QUAN TRỌNG ĐÃ SỬA ==========
        [HttpGet("vnpay-return")]
        public async Task<IActionResult> VNPayReturn()
        {
            // BỌC TOÀN BỘ HÀM TRONG TRY-CATCH
            // ĐỂ BẮT LỖI SẬP SERVER (ERR_EMPTY_RESPONSE)
            string frontendSuccessUrl = "http://localhost:3000/payment-success";
            string frontendFailedUrl = "http://localhost:3000/payment-failed";
            try
            {
                var queryParams = HttpContext.Request.Query;
                var result = await _vnPayService.ValidateRespond(queryParams);

                // Kiểm tra nếu có lỗi null để tránh crash
                if (result == null)
                {
                    //return BadRequest("VNPay result is null");
                    return Redirect(frontendFailedUrl);
                }

                if (result.Status == Const.SUCCESS_PAYMENT_CODE)
                {
                    //return Ok(result);
                    return Redirect(frontendSuccessUrl);
                }
                else
                {
                    return Redirect(frontendFailedUrl);
                    //return BadRequest(result);
                }

            }
            catch (Exception ex)
            {
                // NẾU SERVER SẬP, LỖI SẼ HIỆN Ở ĐÂY
                // Trình duyệt sẽ hiển thị file JSON này thay vì ERR_EMPTY_RESPONSE
                string errorMessage = ex.InnerException?.Message ?? ex.Message;
                Console.WriteLine($"FATAL CRASH in VNPayReturn: {errorMessage}");
                return BadRequest(new ServiceResult(Const.ERROR_EXCEPTION, $"Controller-level error: {errorMessage}"));
            }
        }
    }
}