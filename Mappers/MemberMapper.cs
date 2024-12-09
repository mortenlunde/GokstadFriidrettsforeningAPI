using GokstadFriidrettsforeningAPI.ModelResponses;
using GokstadFriidrettsforeningAPI.Models;

namespace GokstadFriidrettsforeningAPI.Mappers;

public class MemberMapper : IMapper<Member, MemberResponse>
{
    public MemberResponse MapToResonse(Member model)
    {
        return new MemberResponse
        {
            MemberId = model.MemberId,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            Gender = model.Gender,
            Address = model.Address,
            DateOfBirth = model.DateOfBirth,
            Created = model.Created,
            Updated = model.Updated
        };
    }

    public Member MapToModel(MemberResponse response)
    {
        return new Member()
        {
            MemberId = response.MemberId,
            FirstName = response.FirstName,
            LastName = response.LastName,
            Email = response.Email,
            Gender = response.Gender,
            Address = response.Address,
            DateOfBirth = response.DateOfBirth,
            Created = response.Created,
            Updated = response.Updated
        };
    }
}