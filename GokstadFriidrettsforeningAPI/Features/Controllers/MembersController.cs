using GokstadFriidrettsforeningAPI.Features.Services;
using GokstadFriidrettsforeningAPI.Features.Services.Interfaces;
using GokstadFriidrettsforeningAPI.Middleware;
using GokstadFriidrettsforeningAPI.ModelResponses;
using GokstadFriidrettsforeningAPI.TokenHandling;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using UnauthorizedAccessException = System.UnauthorizedAccessException;
namespace GokstadFriidrettsforeningAPI.Features.Controllers;
/// <summary>
/// Controller-laget eksponerer API-endepunktene for medlemmer.
/// Tar imot HTTP-forespørsler, validerer input og delegerer logikk til servicelaget.
/// </summary>

[ApiController]
[Route("api/v1/[controller]")]
public class MembersController(
    ILogger<MembersController> logger,
    IMemberService memberService,
    ITokenService tokenService) : ControllerBase
{
    [Authorize]
    [HttpGet(Name = "GetMembers")]
    public async Task<ActionResult> GetMembers(
        [FromQuery] MemberQuery query, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        if (page <= 0 || pageSize <= 0)
        {
            logger.LogWarning("Ugyldige søkeparametre: sidenummer {Page}, sidestørrelse {PageSize}", page, pageSize);
            return BadRequest(new { Message = "Sideenummer og sidestørrelse må være større enn 0" });
        }

        try
        {
            logger.LogInformation("Henter medlemmer for sidenummer {Page} med sidestørrelse {PageSize}", page, pageSize);
            if (string.IsNullOrWhiteSpace(query.Firstname) && string.IsNullOrWhiteSpace(query.Lastname) && string.IsNullOrWhiteSpace(query.Email))
            {
                var memberResponses = await memberService.GetPagedAsync(page, pageSize);
                if (!memberResponses.Any())
                {
                    logger.LogInformation("Ingen medlemmer for sidenummer {Page} med sidestørrelse {PageSize}", page, pageSize);
                    return NotFound(new { Message = "Ingen medlemmer funnet." });
                }
                logger.LogInformation("Hentet medlemmer på side {Page} med sidestørrelse {PageSize}", page, pageSize);
                return Ok(memberResponses);
            }
            else
            {
                return Ok(await memberService.FindAsync(query));
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "En ukjent feil oppsto ved henting av medlemmer");
            return StatusCode(500, new { Message = "En ukjent feil oppsto. Vennligst prøv igjen senere!" });
        }
    }

    [AllowAnonymous]
    [HttpGet("{id:int}", Name = "GetMember")]
    public async Task<ActionResult<MemberResponse>> GetMemberById(int id)
    {
        var memberResponse = await memberService.GetByIdAsync(id);
        if (memberResponse is null)
            logger.LogError("Medlem med id {id} ikke funnet", id);
        
        return memberResponse is null
            ? NotFound(new { Message = "Ingen medlemmer funnet." })
            : Ok(memberResponse);
    }

    [AllowAnonymous]
    [HttpPost("Register", Name = "RegisterMember")]
    public async Task<ActionResult<MemberResponse>> RegisterMemberAsync(MemberRegistration memberRegistration)
    {
        try
        {
            var member = await memberService.RegisterAsync(memberRegistration);

            if (member == null)
            {
                logger.LogWarning("Registrering mislyktes: {FirstName} {LastName}", memberRegistration.FirstName, memberRegistration.LastName);
                return BadRequest(new { Message = "Registrering mislyktes. Epost kan allerede være brukt, eller feil med inndata." });
            }

            logger.LogInformation("Nytt medlem registrert: {FirstName} {LastName}, Epost: {Email}", member.FirstName, member.LastName, member.Email);
            return Ok(member);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "En ukjent feil oppsto ved registrering");
            return StatusCode(500, new { Message = "En ukjent feil oppsto. Vennligst prøv igjen senere!" });
        }
    }

    [AllowAnonymous]
    [HttpPost("Login")]
    public async Task<ActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            logger.LogInformation("Forsøker innlogging for medlem med epost: {Email}", request.Email);

            var user = await memberService.AuthenticateUserAsync(request.Email, request.Password);
            
            string token = tokenService.GenerateToken(user.MemberId, user.Email);
            logger.LogInformation("Medlem {Email} logget vellykket inn.", request.Email);

            return Ok(new { Token = token });
        }
        catch (UnauthorisedOperation ex)
        {
            logger.LogWarning(ex, "Uautorisert innloggingsforsøk for medlem: {Email}", request.Email);
            return Unauthorized(new { Message = "Feil epost eller passord tastet inn." });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "En ukjent feil oppsto ved forsøk av innlogging for medlem: {Email}", request.Email);
            return StatusCode(500, new { Message = "En ukjent feil oppsto. Vennligst prøv igjen senere!" });
        }
    }

    [Authorize]
    [HttpDelete("Delete/{id:int}", Name = "DeleteMember")]
    public async Task<ActionResult<MemberResponse>> DeleteMemberAsync(int id)
    {
        logger.LogInformation("Bruker forsøker å slette medlemskonto med ID {MemberId}.", id);

        try
        {
            await memberService.DeleteByIdAsync(id);
            return Ok("Bruker slettet!");
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized("Du har ikke tilgang til å slette denne medlemskontoen.");
        }
        catch (Exception ex)
        {
            logger.LogError("En feil oppsto under sletting av medlem: {Message}", ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, "En intern feil oppsto.");
        }
    }

    [Authorize]
    [HttpPut("Update/{id:int}", Name = "UpdateMember")]
    public async Task<ActionResult<MemberResponse>> UpdateMemberAsync(int id, [FromBody] MemberRegistration memberRegistration)
    {
        logger.LogInformation("Oppdaterer bruker med id {id}", id);
        MemberResponse result = await memberService.UpdateMemberAsync(id, memberRegistration);
        
        return Ok(result);
    }
}