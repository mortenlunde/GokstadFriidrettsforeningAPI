using GokstadFriidrettsforeningAPI.Features.Repositories;
using GokstadFriidrettsforeningAPI.Features.Services.Interfaces;
using GokstadFriidrettsforeningAPI.Mappers;
using GokstadFriidrettsforeningAPI.ModelResponses;
using GokstadFriidrettsforeningAPI.Models;

namespace GokstadFriidrettsforeningAPI.Features.Services;

public class MemberService(
    ILogger<Member> logger,
    IMapper<Member, MemberResponse> mapper,
    IMemberRepository repositry,
    IMapper<Member, MemberRegistration> regMapper,
    IHttpContextAccessor httpContextAccessor) : IMemberService
{
    public async Task<IEnumerable<MemberResponse>> GetPagedAsync(int pageNumber, int pageSize)
    {
        IEnumerable<Member> members = await repositry.GetPagedAsync(pageNumber, pageSize);
        return members.Select(mapper.MapToResonse).ToList();
    }
}