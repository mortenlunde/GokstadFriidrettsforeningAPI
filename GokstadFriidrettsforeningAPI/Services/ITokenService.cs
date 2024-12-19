namespace GokstadFriidrettsforeningAPI.Services;

public interface ITokenService
{
    string GenerateToken(int memberId, string email);
}