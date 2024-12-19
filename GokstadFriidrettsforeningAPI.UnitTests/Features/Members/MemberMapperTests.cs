using GokstadFriidrettsforeningAPI.Mappers;
using GokstadFriidrettsforeningAPI.ModelResponses;
using GokstadFriidrettsforeningAPI.Models;
namespace GokstadFriidrettsforeningAPI.UnitTests.Features.Members;

public class MemberMapperTests
{
    private readonly IMapper<Member, MemberResponse> _memberMapper = new MemberMapper();

    [Fact]
    public void MapToDTO_When_UserModelIsValid_Should_Return_UserDTO()
    {
        // Arrange
        Member member = new()
        {
            MemberId = 1,
            Email = "email@email.com",
            FirstName = "Ola",
            LastName = "Normann",
            Gender = 'M',
            Address = new Address
            {
                Street = "Karl Johans gate 1",
                City = "Oslo",
                PostalCode = 0154,
            },
            DateOfBirth = new DateOnly(1990, 01, 01),
            Updated = new DateTime(2024, 12, 23, 9, 45, 00 ),
            Created = new DateTime(2024, 10, 23, 9, 45, 00 ),
            HashedPassword = "fgbacuyrabuyfFYT$%&123" 
        };

        // Act
        MemberResponse memberResponse = _memberMapper.MapToResponse(member);

        // Assert
        Assert.NotNull(memberResponse);
        Assert.Equal(member.MemberId, memberResponse.MemberId);
        Assert.Equal(member.Email, memberResponse.Email);
        Assert.Equal(member.FirstName, memberResponse.FirstName);
        Assert.Equal(member.LastName, memberResponse.LastName);
        Assert.Equal(member.Gender, memberResponse.Gender);
        Assert.Equal(member.DateOfBirth, memberResponse.DateOfBirth);
        Assert.Equal(member.Address, memberResponse.Address);
        Assert.Equal(member.Updated, memberResponse.Updated);
        Assert.Equal(member.Created, memberResponse.Created);
    }

    [Fact]
    public void MapToModel_When_UserDTOIsValid_Should_Return_User()
    {
        // Arrange
        MemberResponse response = new()
        {
            MemberId = 1,
            Email = "email@email.com",
            FirstName = "Ola",
            LastName = "Normann",
            Gender = 'M',
            Address = new Address
            {
                Street = "Karl Johans gate 1",
                City = "Oslo",
                PostalCode = 0154,
            },
            DateOfBirth = new DateOnly(1990, 01, 01),
            Created = new DateTime(2024, 10, 23, 9, 45, 00),
            Updated = new DateTime(2024, 12, 23, 9, 45, 00),
        };
        
        // Act
        Member member = _memberMapper.MapToModel(response);
        
        // Assert
        Assert.NotNull(member);
        Assert.Equal(response.MemberId, member.MemberId);
        Assert.Equal(response.Email, member.Email);
        Assert.Equal(response.FirstName, member.FirstName);
        Assert.Equal(response.Address, member.Address);
        Assert.Equal(response.DateOfBirth, member.DateOfBirth);
        Assert.Equal(response.Gender, member.Gender);
        Assert.Equal(response.LastName, member.LastName);
        Assert.Equal(response.Updated, member.Updated);
        Assert.Equal(response.Created, member.Created);
    }
}