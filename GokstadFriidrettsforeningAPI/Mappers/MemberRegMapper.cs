using GokstadFriidrettsforeningAPI.ModelResponses;
using GokstadFriidrettsforeningAPI.Models;

namespace GokstadFriidrettsforeningAPI.Mappers;

public class MemberRegMapper : IMapper<Member, MemberRegistration>
{
    public MemberRegistration MapToResponse(Member model)
    {
        return new MemberRegistration
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            Gender = model.Gender,
            Address = model.Address,
            DateOfBirth = model.DateOfBirth,
        };
    }

    public Member MapToModel(MemberRegistration response)
    {
        return new Member
        {
            MemberId = 0,
            FirstName = response.FirstName,
            LastName = response.LastName,
            Email = response.Email,
            Gender = response.Gender,
            DateOfBirth = response.DateOfBirth,
            Address = response.Address,
        };
    }
}