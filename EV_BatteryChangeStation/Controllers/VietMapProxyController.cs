using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VietMapProxyController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "YOUR_VIETMAP_API_KEY"; // thay bằng key của bạn

        public VietMapProxyController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet("geocode")]
        public async Task<IActionResult> Geocode([FromQuery] string text)
        {
            if (string.IsNullOrEmpty(text))
                return BadRequest("Missing text param.");

            var url = $"https://maps.vietmap.vn/api/search/v4?apikey={_apiKey}&text={Uri.EscapeDataString(text)}";
            try
            {
                var res = await _httpClient.GetAsync(url);
                var content = await res.Content.ReadAsStringAsync();
                return Content(content, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}