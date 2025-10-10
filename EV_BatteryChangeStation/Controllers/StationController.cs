using EV_BatteryChangeStation_Common.DTOs.StationDTO;
using EV_BatteryChangeStation_Service.InternalService.IService;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class StationController : ControllerBase
{
    private readonly IStationService _stationService;

    public StationController(IStationService stationService)
    {
        _stationService = stationService;
    }

    [HttpPost("Create/")]
    public async Task<IActionResult> CreateStation([FromBody] StationCreateDTO dto)
    {
        var result = await _stationService.CreateAsync(dto);
        if (result.Status != 200)
            return BadRequest(result);

        var createdStation = result.Data as StationDTO;
        return CreatedAtAction(nameof(GetStation), new { id = createdStation?.StationId }, createdStation);
    }

    [HttpPut("Update/{id}")]
    public async Task<IActionResult> UpdateStation(int id, [FromBody] StationCreateDTO dto)
    {
        var result = await _stationService.UpdateAsync(id, dto);
        if (result.Status == 404) return NotFound(result);
        if (result.Status != 200) return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> DeleteStation(int id)
    {
        var result = await _stationService.DeleteAsync(id);
        if (result.Status == 404) return NotFound(result);
        if (result.Status != 200) return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("SelectAll/")]
    public async Task<IActionResult> GetStations()
    {
        var result = await _stationService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("Select/{id}")]
    public async Task<IActionResult> GetStation(int id)
    {
        var result = await _stationService.GetByIdAsync(id);
        if (result.Status == 404) return NotFound(result);
        return Ok(result);
    }

    [HttpDelete("HardDelete/{id}")]
    public async Task<IActionResult> HardDeleteStation(int id)
    {
        var result = await _stationService.HardDeleteAsync(id);
        if (result.Status == 404) return NotFound(result);
        if (result.Status != 200) return BadRequest(result);

        return Ok(result);
    }
}
