using GokstadFriidrettsforeningAPI.Data;
using GokstadFriidrettsforeningAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace GokstadFriidrettsforeningAPI.Features.Controllers;
/// <summary>
/// Controller-laget eksponerer API-endepunktene.
/// Tar imot HTTP-forespørsler, validerer input og delegerer logikk til servicelaget.
/// </summary>

[ApiController]
[Route("api/[controller]")]
public class LogsController(GaaDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Log>>> GetLogsAsync([FromQuery] string? level)
    {
        try
        {
            IQueryable<Log> query = context.Logs;

            if (!string.IsNullOrEmpty(level))
            {
                query = query.Where(log => log.Level == level);
            }

            var logs = await query
                .Select(log => new Log
                {
                    Id = log.Id,
                    Timestamp = log.Timestamp,
                    Level = log.Level,
                    Message = log.Message,
                    Exception = log.Exception
                })
                .ToListAsync();

            if (!logs.Any())
            {
                return NotFound(new { Message = "Ingen logger funnet." });
            }

            return Ok(logs);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "En feil oppsto ved henting av logger.", Details = ex.Message });
        }
    }
}