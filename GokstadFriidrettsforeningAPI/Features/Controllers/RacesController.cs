using GokstadFriidrettsforeningAPI.Features.Services;
using GokstadFriidrettsforeningAPI.Features.Services.Interfaces;
using GokstadFriidrettsforeningAPI.ModelResponses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace GokstadFriidrettsforeningAPI.Features.Controllers;
/// <summary>
/// Controller-laget eksponerer API-endepunktene.
/// Tar imot HTTP-forespørsler, validerer input og delegerer logikk til servicelaget.
/// </summary>

[ApiController]
[Route("api/v1/[controller]")]
public class RacesController(ILogger<MembersController> logger,
    IRaceService raceService) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet(Name = "GetRaces")]
    public async Task<ActionResult> GetRacesAsync(
        [FromQuery] RacesQuery query, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        if (page <= 0 || pageSize <= 0)
        {
            logger.LogWarning("Ugyldige søkeparametre: sidenummer {Page}, sidestørrelse {PageSize}", page, pageSize);
            return BadRequest(new { Message = "Sideenummer og sidestørrelse må være større enn 0" });
        }

        try
        {
            logger.LogInformation("Henter løp for sidenummer {Page} med sidestørrelse {PageSize}", page, pageSize);
            if (string.IsNullOrWhiteSpace(query.RaceName) && string.IsNullOrWhiteSpace(query.Date.ToString()) && string.IsNullOrWhiteSpace(query.Distance.ToString()) && string.IsNullOrWhiteSpace(query.Laps.ToString()))
            {
                var raceResponses = await raceService.GetPagedAsync(page, pageSize);
                if (!raceResponses.Any())
                {
                    logger.LogInformation("Ingen løp for sidenummer {Page} med sidestørrelse {PageSize}", page, pageSize);
                    return NotFound(new { Message = "Ingen løp funnet." });
                }
                logger.LogInformation("Hentet løp på side {Page} med sidestørrelse {PageSize}", page, pageSize);
                return Ok(raceResponses);
            }
            else
            {
                return Ok(await raceService.FindAsync(query));
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "En ukjent feil oppsto ved henting av løp");
            return StatusCode(500, new { Message = "En ukjent feil oppsto ved henting av løp. Vennligst prøv igjen senere!" });
        }
    }

    [AllowAnonymous]
    [HttpPost("Create", Name = "RegisterRaces")]
    public async Task<ActionResult<RaceResponse>> AddRacesAsync(RaceResponse raceResponse)
    {
        try
        {
            var race = await raceService.RegisterAsync(raceResponse);

            if (race == null)
            {
                logger.LogWarning("Registrering mislyktes: {name}", raceResponse.RaceName);
                return BadRequest(new { Message = "Registrering mislyktes." });
            }

            logger.LogInformation("Nytt løp registrert: {name}", race.RaceName);
            return Ok(race);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "En ukjent feil oppsto ved registrering");
            return StatusCode(500, new { Message = "En ukjent feil oppsto. Vennligst prøv igjen senere!" });
        }
    }
    
    [HttpDelete("Delete")]
    public async Task<ActionResult> DeleteRaceAsync([FromQuery] int id)
    {
        logger.LogInformation("Bruker forsøker å slette løp med ID {raceId}.", id);

        try
        {
            await raceService.DeleteByIdAsync(id);
            
            return Ok("Løp slettet!");
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized("Du har ikke tilgang til å slette dette løpet.");
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogError(ex, "Løp ble ikke funnet under sletting: Løp {RaceId}", id);
            return NotFound(new { ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError("En feil oppsto under sletting av løp: {Message}", ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, "En intern feil oppsto.");
        }
    }
}