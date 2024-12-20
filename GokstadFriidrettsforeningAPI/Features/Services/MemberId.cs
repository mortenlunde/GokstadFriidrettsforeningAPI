using System.Security.Claims;
namespace GokstadFriidrettsforeningAPI.Features.Services;
/// <summary>
/// Henter ut MemberID for å sjekke autorisasjon
/// </summary>

public interface IUserContextService
{
    int? GetMemberId();
}

public class UserContextService(IHttpContextAccessor httpContextAccessor) : IUserContextService
{
    public int? GetMemberId()
    {
        Claim? memberIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("MemberId");
        if (memberIdClaim == null)
            throw new UnauthorizedAccessException("Member ID is missing in the token.");

        return int.Parse(memberIdClaim.Value);
    }
}
