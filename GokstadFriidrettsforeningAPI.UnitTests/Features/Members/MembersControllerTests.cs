using GokstadFriidrettsforeningAPI.Features.Controllers;
using GokstadFriidrettsforeningAPI.Features.Services;
using GokstadFriidrettsforeningAPI.Features.Services.Interfaces;
using GokstadFriidrettsforeningAPI.ModelResponses;
using GokstadFriidrettsforeningAPI.Models;
using GokstadFriidrettsforeningAPI.TokenHandling;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
namespace GokstadFriidrettsforeningAPI.UnitTests.Features.Members;

public class MembersControllerTests
{
    private readonly MembersController _membersController;
    private readonly Mock<ILogger<MembersController>> _mockLogger  = new();
    private readonly Mock<IMemberService> _mockmemberService = new();
    private readonly Mock<ITokenService> _mockTokenService = new();

    public MembersControllerTests()
    {
        _membersController = new MembersController(_mockLogger.Object, _mockmemberService.Object, _mockTokenService.Object);
    }
    
    [Fact]
    public async Task GetmembersAsync_WhenDefaultPageSizeAndOneUserExists_ShouldReturnOneUser()
    {
        // Arrange
        const string lastName = "Normann";
        List<MemberResponse> dtos =
        [
            new ()
            {
                MemberId = 1,
                FirstName = "Ola",
                LastName = "Normann",
                Gender = 'M',
                Address = new Address
                {
                    Street = "Karl Johans gate 1",
                    City = "Oslo",
                    PostalCode = 0154,
                },
                DateOfBirth = new DateOnly(1981, 10, 01),
                Email = "ola_normann@gmail.com",
                Updated = DateTime.UtcNow,
                Created = DateTime.UtcNow,
            }
        ];
    
        _mockmemberService.Setup(x => 
                x.GetPagedAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(dtos);
    
        // Act
        ActionResult<IEnumerable<MemberResponse>> result = await _membersController.GetMembers(new MemberQuery());

        // Assert
        ActionResult<IEnumerable<MemberResponse>> actionResult = Assert.IsType<ActionResult<IEnumerable<MemberResponse>>>(result);
        OkObjectResult returnValue = Assert.IsType<OkObjectResult>(actionResult.Result);
        List<MemberResponse> memberResponses = Assert.IsType<List<MemberResponse>>(returnValue.Value);
        MemberResponse? member = memberResponses.FirstOrDefault();
        Assert.NotNull(member);
        Assert.Equal(lastName, member.LastName);
    }
}