using GokstadFriidrettsforeningAPI.ModelResponses;
using GokstadFriidrettsforeningAPI.Models;

namespace GokstadFriidrettsforeningAPI.Mappers;

public class MemberRegMapper : IMapper<Member, MemberRegistration>
{
    public MemberRegistration MapToResonse(Member model)
    {
        return new MemberRegistration()
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
        };
    }

    public Member MapToModel(MemberRegistration response)
    {
        return new Member
        {
            FirstName = response.FirstName,
            LastName = response.LastName,
            Email = response.Email,
        };
    }
}