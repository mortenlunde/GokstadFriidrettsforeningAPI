using GokstadFriidrettsforeningAPI.Features.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace GokstadFriidrettsforeningAPI.Features.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class RegistrationsController(ILogger<MembersController> logger,
    IRegistrationService registrationService) : ControllerBase
{
    [Authorize]
    [HttpPost("Register")]
    public async Task<IActionResult> RegisterForRaceAsync([FromBody] RegistrationRequest request)
    {
        try
        {
            var registration = await registrationService.RegisterAsync(request.MemberId, request.RaceId);

            if (registration == null)
            {
                logger.LogWarning("Registrering mislyktes: Medlem {MemberId}, Løp {RaceId}", request.MemberId,
                    request.RaceId);
                return BadRequest(new { Message = "Registrering mislyktes." });
            }

            logger.LogInformation("Ny aktivitet registrert: Medlem {MemberId} til løp {RaceId}", registration.MemberId,
                registration.RaceId);
            return Ok(registration);
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogError(ex, "Løp ble ikke funnet under registrering: Løp {RaceId}", request.RaceId);
            return NotFound(new { Message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { Message = "Du har ikke tilgang til å registrere løp for denne medlemskontoen." });
        }
        catch (InvalidOperationException)
        {
            return BadRequest("Du er allerede registrert for dette løpet.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "En ukjent feil oppsto ved registrering.");
            return StatusCode(500, new { Message = "En ukjent feil oppsto. Vennligst prøv igjen senere!" });
        }
    }
    
    public class RegistrationRequest
    {
        public int MemberId { get; set; }
        public int RaceId { get; set; }
    }


    [HttpDelete("Unregister")]
    public async Task<IActionResult> UnregisterFromRaceAsync([FromBody] RegistrationRequest request)
    {
        logger.LogInformation("Bruker forsøker å slette medlemskonto med ID {MemberId}.", request.MemberId);

        try
        {
            await registrationService.DeleteRegistrationAsync(request.MemberId, request.RaceId);
            
            return Ok("Aktivitet slettet!");
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized("Du har ikke tilgang til å slette denne medlemskontoen.");
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogError(ex, "Løp ble ikke funnet under registrering: Løp {RaceId}", request.RaceId);
            return NotFound(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError("En feil oppsto under sletting av medlem: {Message}", ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, "En intern feil oppsto.");
        }
    }
}