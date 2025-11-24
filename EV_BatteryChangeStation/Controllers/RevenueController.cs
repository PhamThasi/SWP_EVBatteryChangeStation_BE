using EV_BatteryChangeStation_Service.InternalService.IService;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/report")]
public class ReportController : ControllerBase
{
    private readonly IRevenueService _revenueService;

    public ReportController(IRevenueService revenueService)
    {
        _revenueService = revenueService;
    }

    [HttpGet("revenue-by-station")]
    public async Task<IActionResult> GetRevenueByStation()
    {
        var result = await _revenueService.GetRevenueByStationAsync();
        return Ok(result);
    }
}
