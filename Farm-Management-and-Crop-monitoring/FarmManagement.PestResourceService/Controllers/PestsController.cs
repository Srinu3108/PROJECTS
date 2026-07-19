using FarmManagement.Web.Data;
using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models.Enums;
using FarmManagement.Web.States;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FarmManagement.PestResourceService.Controllers;

[ApiController]
[Route("api/pests")]
[Authorize]
public class PestsController : ControllerBase
{
    private readonly FarmDbContext _db;
    public PestsController(FarmDbContext db) => _db = db;

    /// <summary>Get all pest incidents.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? status)
    {
        var query = _db.PestIncidents.Include(p => p.Crop).AsNoTracking();
        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<IncidentStatus>(status, true, out var parsed))
            query = query.Where(p => p.Status == parsed);
        return Ok(await query.OrderByDescending(p => p.ReportedDate).ToListAsync());
    }

    /// <summary>Get a pest incident by ID.</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var pest = await _db.PestIncidents.Include(p => p.Crop).Include(p => p.Resources)
                            .AsNoTracking().FirstOrDefaultAsync(p => p.PestIncidentId == id);
        return pest is null ? NotFound(new { message = $"Incident {id} not found." }) : Ok(pest);
    }

    /// <summary>Report a new pest incident.</summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Farmer,FieldSupervisor,Agronomist")]
    public async Task<IActionResult> Create([FromBody] PestRequest req)
    {
        var pest = new PestIncident
        {
            PestName     = req.PestName,
            DiseaseName  = req.DiseaseName,
            Description  = req.Description ?? string.Empty,
            CropId       = req.CropId,
            ReportedDate = DateTime.Now,
            Status       = IncidentStatus.Active
        };
        _db.PestIncidents.Add(pest);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = pest.PestIncidentId }, pest);
    }

    /// <summary>Update a pest incident's status using the State Machine (Active → Monitoring → Resolved).</summary>
    [HttpPatch("{id:int}/status")]
    [Authorize(Roles = "Admin,Farmer,FieldSupervisor,Agronomist")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] StatusRequest req)
    {
        var pest = await _db.PestIncidents.FindAsync(id);
        if (pest is null) return NotFound(new { message = $"Incident {id} not found." });

        try
        {
            PestStateMachine.Transition(pest, req.Status);
            await _db.SaveChangesAsync();
            return Ok(new { message = $"Status updated to {pest.Status}.", currentStatus = pest.Status.ToString() });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>Delete a pest incident.</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,Farmer")]
    public async Task<IActionResult> Delete(int id)
    {
        var pest = await _db.PestIncidents.FindAsync(id);
        if (pest is null) return NotFound(new { message = $"Incident {id} not found." });
        _db.PestIncidents.Remove(pest);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

public record PestRequest(string PestName, string? DiseaseName, string? Description, int CropId);
public record StatusRequest(string Status);
