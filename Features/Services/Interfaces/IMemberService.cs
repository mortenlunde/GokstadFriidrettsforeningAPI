using GokstadFriidrettsforeningAPI.ModelResponses;
using GokstadFriidrettsforeningAPI.Models;
namespace GokstadFriidrettsforeningAPI.Features.Services.Interfaces;

public interface IMemberService : IService<MemberResponse>
{
    Task<MemberResponse?> RegisterAsync(MemberRegistration regResponse);
    Task<Member> AuthenticateUserAsync(string username, string password);
    Task<MemberResponse> UpdateMemberAsync(int id, MemberRegistration entity);
    Task<IEnumerable<MemberResponse>> FindAsync(MemberQuery searchQuery);
}