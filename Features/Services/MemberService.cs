using System.Linq.Expressions;
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

    public async Task<MemberResponse?> RegisterAsync(MemberRegistration regResponse)
    {
        Member member = regMapper.MapToModel(regResponse);
        member.Created = DateTime.UtcNow;
        member.Updated = DateTime.UtcNow;
        member.HashedPassword = BCrypt.Net.BCrypt.HashPassword(regResponse.Password);
        
        Member? userResponse = await repositry.AddAsync(member);
        return userResponse is null
            ? null
            : mapper.MapToResonse(userResponse);
    }
    
    public async Task<Member> AuthenticateUserAsync(string email, string password)
    {
        Expression<Func<Member, bool>> expression = user => user.Email == email;
        Member? usr = (await repositry.FindAsync(expression)).FirstOrDefault();
    
        if (usr is null || !BCrypt.Net.BCrypt.Verify(password, usr.HashedPassword))
            return null;

        return usr;
    }
}