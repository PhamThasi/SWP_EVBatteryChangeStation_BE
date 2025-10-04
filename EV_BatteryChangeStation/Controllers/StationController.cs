using EV_BatteryChangeStation_Common.DTOs.StationDTO;
using EV_BatteryChangeStation_Repository.DBContext;
using EV_BatteryChangeStation_Repository.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class StationController : ControllerBase
{
    private readonly EvbatterySwapContext _context;

    public StationController(EvbatterySwapContext context)
    {
        _context = context;
    }

    // GET: api/Station
    [HttpGet]
    public async Task<ActionResult<IEnumerable<StationDTO>>> GetStations()
    {
        return await _context.Stations
            .Select(s => new StationDTO
            {
                StationId = s.StationId,
                Address = s.Address,
                PhoneNumber = s.PhoneNumber,
                Status = s.Status,
                AccountName = s.AccountName,
                BatteryQuality = s.BatteryQuality
            })
            .ToListAsync();
    }

    // GET: api/Station/5
    [HttpGet("{id}")]
    public async Task<ActionResult<StationDTO>> GetStation(int id)
    {
        var station = await _context.Stations.FindAsync(id);
        if (station == null) return NotFound();

        return new StationDTO
        {
            StationId = station.StationId,
            Address = station.Address,
            PhoneNumber = station.PhoneNumber,
            Status = station.Status,
            AccountName = station.AccountName,
            BatteryQuality = station.BatteryQuality
        };
    }

    // POST: api/Station
    [HttpPost]
    public async Task<ActionResult<StationDTO>> CreateStation(StationDTO dto)
    {
        var station = new Station
        {
            Address = dto.Address,
            PhoneNumber = dto.PhoneNumber,
            Status = dto.Status,
            AccountName = dto.AccountName,
            BatteryQuality = dto.BatteryQuality
        };

        _context.Stations.Add(station);
        await _context.SaveChangesAsync();

        dto.StationId = station.StationId;
        return CreatedAtAction(nameof(GetStation), new { id = station.StationId }, dto);
    }

    // PUT: api/Station/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStation(int id, StationDTO dto)
    {
        if (id != dto.StationId) return BadRequest();

        var station = await _context.Stations.FindAsync(id);
        if (station == null) return NotFound();

        station.Address = dto.Address;
        station.PhoneNumber = dto.PhoneNumber;
        station.Status = dto.Status;
        station.AccountName = dto.AccountName;
        station.BatteryQuality = dto.BatteryQuality;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/Station/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStation(int id)
    {
        var station = await _context.Stations.FindAsync(id);
        if (station == null) return NotFound();

        _context.Stations.Remove(station);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
