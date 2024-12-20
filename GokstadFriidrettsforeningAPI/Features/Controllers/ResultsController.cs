using GokstadFriidrettsforeningAPI.Features.Services;
using GokstadFriidrettsforeningAPI.Features.Services.Interfaces;
using GokstadFriidrettsforeningAPI.ModelResponses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace GokstadFriidrettsforeningAPI.Features.Controllers;
/// <summary>
/// Controller-laget eksponerer API-endepunktene for medlemmer.
/// Tar imot HTTP-forespørsler, validerer input og delegerer logikk til servicelaget.
/// </summary>

[ApiController]
[Route("api/v1/[controller]")]
public class ResultsController(ILogger<MembersController> logger,
    IResultService resultService) : ControllerBase
{
    [Authorize]
    [HttpGet(Name = "GetResults")]
    public async Task<IActionResult> GetResultsAsync(
        [FromQuery] ResultQuery query, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        if (page <= 0 || pageSize <= 0)
        {
            logger.LogWarning("Ugyldige søkeparametre: sidenummer {Page}, sidestørrelse {PageSize}", page, pageSize);
            return BadRequest(new { Message = "Sideenummer og sidestørrelse må være større enn 0" });
        }

        try
        {
            logger.LogInformation("Henter resultater for sidenummer {Page} med sidestørrelse {PageSize}", page, pageSize);
            if (string.IsNullOrWhiteSpace(query.MemberId.ToString()) && string.IsNullOrWhiteSpace(query.RaceId.ToString()) && string.IsNullOrWhiteSpace(query.Time.ToString()))
            {
                var memberResponses = await resultService.GetPagedAsync(page, pageSize);
                if (!memberResponses.Any())
                {
                    logger.LogInformation("Ingen resultater for sidenummer {Page} med sidestørrelse {PageSize}", page, pageSize);
                    return NotFound(new { Message = "Ingen resultater funnet." });
                }
                logger.LogInformation("Hentet resultater på side {Page} med sidestørrelse {PageSize}", page, pageSize);
                return Ok(memberResponses);
            }
            else
            {
                return Ok(await resultService.FindAsync(query));
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "En ukjent feil oppsto ved henting av resultater");
            return StatusCode(500, new { Message = "En ukjent feil oppsto. Vennligst prøv igjen senere!" });
        }
    }
    
    [Authorize]
    [HttpPost("Register")]
    public async Task<IActionResult> RegisterResultAsync([FromBody] ResultResponse request)
    {
        try
        {
            var registration = await resultService.RegisterAsync(request.MemberId, request.RaceId, request.Time.ToString());

            if (registration == null)
            {
                logger.LogWarning("Registrering av resultat mislyktes: Medlem {MemberId}, Løp {RaceId}", request.MemberId,
                    request.RaceId);
                return BadRequest(new { Message = "Registrering mislyktes." });
            }

            logger.LogInformation("Nytt resultat registrert: Medlem {MemberId} til løp {RaceId}", registration.MemberId,
                registration.RaceId);

            return Ok(registration);
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogError(ex, "Løp ble ikke funnet under registrering av resultat: Løp {RaceId}", request.RaceId);
            return NotFound(new { ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { Message = "Du har ikke tilgang til å registrere resultat for denne medlemskontoen." });
        }
        catch (InvalidOperationException)
        {
            return BadRequest("Du er allerede registrert resultat for dette løpet.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "En ukjent feil oppsto ved registrering.");
            return StatusCode(500, new { Message = "En ukjent feil oppsto. Vennligst prøv igjen senere!" });
        }
    }
    
    [Authorize]
    [HttpDelete("Delete")]
    public async Task<IActionResult> DeleteResultAsync([FromBody] ResultDeleteResponse request)
    {
        try
        {
            var result = await resultService.DeleteResultAsync(request.MemberId, request.RaceId);

            if (result == null)
            {
                logger.LogWarning("Sletting av resultat mislyktes: Medlem {MemberId}, Løp {RaceId}", request.MemberId,
                    request.RaceId);
                return BadRequest(new { Message = "Sletting mislyktes." });
            }

            logger.LogInformation("Resultat slettet: Medlem {MemberId} til løp {RaceId}", result.MemberId,
                result.RaceId);

            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogError(ex, "Løp ble ikke funnet under sletting av resultat: Løp {RaceId}", request.RaceId);
            return NotFound(new { ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { Message = "Du har ikke tilgang til å slette resultat for denne medlemskontoen." });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "En ukjent feil oppsto ved sletting av resultat.");
            return StatusCode(500, new { Message = "En ukjent feil oppsto. Vennligst prøv igjen senere!" });
        }
    }
}