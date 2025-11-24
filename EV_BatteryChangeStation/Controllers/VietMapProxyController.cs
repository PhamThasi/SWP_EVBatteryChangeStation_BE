using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors; // hiển_: thêm để có thể bật [EnableCors]
using System.Net.Http;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation.Controllers
{
    [EnableCors("AllowFrontend")] // hiển_: bật policy cho controller này
    [Route("api/[controller]")]
    [ApiController]
    public class VietMapProxyController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey; // hiển_: chuyển từ hardcode sang config để bảo mật

        // hiển_: inject IConfiguration để đọc key từ appsettings.json
        public VietMapProxyController(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _apiKey = config["VietMap:ApiKey"]; 
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
                
                // hiển_: trả nguyên JSON response từ VietMap
                return Content(content, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
