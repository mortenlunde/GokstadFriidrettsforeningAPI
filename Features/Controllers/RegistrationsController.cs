using GokstadFriidrettsforeningAPI.Features.Services.Interfaces;
using GokstadFriidrettsforeningAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace GokstadFriidrettsforeningAPI.Features.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class RegistrationsController(ILogger<MembersController> logger,
    IRegistrationService registrationService,
    ITokenService tokenService) : ControllerBase
{
    [Authorize]
    [HttpPost("register")]
    public async Task<IActionResult> RegisterForRaceAsync([FromBody] RegistrationRequest request)
    {
        try
        {
            var registration = await registrationService.RegisterAsync(request.MemberId, request.RaceId);

            if (registration == null)
            {
                logger.LogWarning("Registrering mislyktes: Member {MemberId} til løp {RaceId}", request.MemberId, request.RaceId);
                return BadRequest(new { Message = "Registrering mislyktes." });
            }

            logger.LogInformation("Ny aktivitet registrert: Member {MemberId} til løp {RaceId}", registration.MemberId, registration.RaceId);
            return Ok(registration);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized("Du har ikke tilgang til å registrere løp for denne medlemskontoen.");
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


    [HttpDelete("unregister")]
    public async Task<IActionResult> UnregisterFromRaceAsync(int memberId, int raceId)
    {
        return Ok();
    }
}