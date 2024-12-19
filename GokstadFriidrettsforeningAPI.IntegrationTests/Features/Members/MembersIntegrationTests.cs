using System.Linq.Expressions;
using System.Net;
using GokstadFriidrettsforeningAPI.ModelResponses;
using GokstadFriidrettsforeningAPI.Models;
using Moq;
using Newtonsoft.Json;

namespace GokstadFriidrettsforeningAPI.IntegrationTests.Features.Members;

public class MembersIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;
    
    public MembersIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }
    
    [Fact]
    public async Task GetUsers_WhenNoSearchParams_ThenReturnsPagedUsers()
    {
        // Arrange
        List<Member> members =
        [
            new()
            {
                MemberId = 1,
                FirstName = "Morten", 
                LastName = "Lunde", 
                Email = "morten@lunde.no",
                Gender = 'M', 
                Address = new Address {City = "Undrumsdal", PostalCode = 3176, Street = "Rødsåsveien 10C"},
                DateOfBirth = new DateOnly(1990,10,30),
                Created = new DateTime(2024,12,12,23,33,49,946),
                Updated = new DateTime(2024,12,12,23,33,49,947),
                HashedPassword = "$2a$11$t1r/bdrPRZiGKqmAtfbLoeI/x9kll6YsRZtu9A31/opmUj7It/88K",
            },
            new()
            {
                FirstName = "Kari", LastName = "Normann", Email = "kari@gmail.com",
                MemberId = 1,
                Gender = 'F', DateOfBirth = new DateOnly(1990,10,30),
                Address = new Address {City = "Larvik", PostalCode = 3170, Street = "Lunde"},
                HashedPassword = "$2a$11$t1r/bdrPRZiGKqmAtfbLoeI/x9kll6YsRZtu9A31/opmUj7It/88K",
                Created = new DateTime(2023, 11, 14, 9, 30, 0),
                Updated = new DateTime(2020, 11, 14, 9, 30, 0),
            }
        ];
        
        _client.DefaultRequestHeaders.Add("Authorization", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJtb3J0ZW5AbHVuZGUubm8iLCJNZW1iZXJJZCI6IjEiLCJqdGkiOiIyNzYyYTk4Ny1lNTM2LTQxYmEtOTk3Ni04Zjg3Yzc2M2E4ODEiLCJleHAiOjE3MzQ1NzAzMDEsImlzcyI6Ikdva3N0YWRGcmlpZHJldHRzZm9yZW5pbmciLCJhdWQiOiJHb2tzdGFkRnJpaWRyZXR0c2ZvcmVuaW5nTWVtYmVycyJ9.u_wCZhZCqwUI-0LPwb08IbKEDT3SMslClJHDrfLzV70");
        
        _factory.MemberRepositoryMock.Setup(x =>
            x.FindAsync(It.IsAny<Expression<Func<Member, bool>>>()))
            .ReturnsAsync(new List<Member>(){members[0]});
        
        _factory.MemberRepositoryMock.Setup(x =>
                x.GetPagedAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(members);
        
        
        // Act
        var response = await _client.GetAsync("/api/v1/Members");
        
        
        // Assert
        var memberResponses = JsonConvert
            .DeserializeObject<IEnumerable<MemberResponse>>(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(memberResponses);
        Assert.Collection(memberResponses,
            m =>
            {
                Assert.Equal(m.Email, members[0].Email);
                Assert.Equal(m.Gender, members[0].Gender);
                Assert.Equal(m.DateOfBirth, members[0].DateOfBirth);
                Assert.Equal(m.Created, members[0].Created);
                Assert.Equal(m.Updated, members[0].Updated);
                Assert.Equal(m.FirstName, members[0].FirstName);
                Assert.Equal(m.LastName, members[0].LastName);
                Assert.Equal(members[0].Address!.City, m.Address!.City);
                Assert.Equal(members[0].Address!.Street, m.Address.Street);
                Assert.Equal(members[0].Address!.PostalCode, m.Address.PostalCode);
            },
            m =>
            {
                Assert.Equal(m.Email, members[1].Email);
                Assert.Equal(m.Gender, members[1].Gender);
                Assert.Equal(members[1].Address!.City, m.Address!.City);
                Assert.Equal(members[1].Address!.Street, m.Address.Street);
                Assert.Equal(members[1].Address!.PostalCode, m.Address.PostalCode);
                Assert.Equal(m.Created, members[1].Created);
                Assert.Equal(m.MemberId, members[1].MemberId);
                Assert.Equal(m.DateOfBirth, members[1].DateOfBirth);
                Assert.Equal(m.Updated, members[1].Updated);
                Assert.Equal(m.FirstName, members[1].FirstName);
                Assert.Equal(m.LastName, members[1].LastName);   
            });
    }
}