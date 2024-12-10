using GokstadFriidrettsforeningAPI.ModelResponses;
using GokstadFriidrettsforeningAPI.Models;

namespace GokstadFriidrettsforeningAPI.Features.Services.Interfaces;

public interface IMemberService : IService<MemberResponse>
{
    Task<MemberResponse?> RegisterAsync(MemberRegistration regResponse);
    Task<Member> AuthenticateUserAsync(string username, string password);
}