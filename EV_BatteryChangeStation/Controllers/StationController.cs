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
    private readonly EVBatterySwapContext _context;
    private readonly IStationService _stationService;

    public StationController(IStationService stationService)
    {
        _stationService = stationService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateStation(StationDTO dto)
    {
        var result = await _stationService.CreateAsync(dto);
        if (result.Status != 200)
            return BadRequest(result);

        // Ép kiểu data về StationDTO nếu có
        var createdStation = result.Data as StationDTO;
        return CreatedAtAction(nameof(GetStation), new { id = createdStation?.StationId }, createdStation);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStation(int id, StationDTO dto)
    {
        if (id != dto.StationId) return BadRequest();

        var result = await _stationService.UpdateAsync(dto);
        if (result.Status == 404) return NotFound(result);
        if (result.Status != 200) return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStation(int id)
    {
        var result = await _stationService.DeleteAsync(id);
        if (result.Status == 404) return NotFound(result);
        if (result.Status != 200) return BadRequest(result);

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetStations()
    {
        var result = await _stationService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetStation(int id)
    {
        var result = await _stationService.GetByIdAsync(id);
        if (result.Status == 404) return NotFound(result);
        return Ok(result);
    }

}
