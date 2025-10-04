using EV_BatteryChangeStation_Common.DTOs.StationDTO;
using EV_BatteryChangeStation_Repository.DBContext;
using EV_BatteryChangeStation_Repository.Entities;
using EV_BatteryChangeStation_Service.InternalService.IService;
using EV_BatteryChangeStation_Service.InternalService.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class StationController : ControllerBase
{
    private readonly EvbatterySwapContext _context;
    private readonly IStationService _stationService;

    public StationController(IStationService stationService)
    {
        _stationService = stationService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StationDTO>>> GetStations() =>
        Ok(await _stationService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<StationDTO>> GetStation(int id)
    {
        var station = await _stationService.GetByIdAsync(id);
        if (station == null) return NotFound();
        return Ok(station);
    }

    [HttpPost]
    public async Task<ActionResult<StationDTO>> CreateStation(StationDTO dto)
    {
        var created = await _stationService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetStation), new { id = created.StationId }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStation(int id, StationDTO dto)
    {
        if (id != dto.StationId) return BadRequest();
        var updated = await _stationService.UpdateAsync(dto);
        if (!updated) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStation(int id)
    {
        var deleted = await _stationService.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
