namespace GokstadFriidrettsforeningAPI.TokenHandling;

public interface ITokenService
{
    string GenerateToken(int memberId, string email);
}