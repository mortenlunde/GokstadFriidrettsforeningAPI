namespace GokstadFriidrettsforeningAPI.Features.Services;

public interface IUserContextService
{
    int GetMemberId();
}

public class UserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int GetMemberId()
    {
        var memberIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("MemberId");
        if (memberIdClaim == null)
            throw new UnauthorizedAccessException("Member ID is missing in the token.");

        return int.Parse(memberIdClaim.Value);
    }
}
