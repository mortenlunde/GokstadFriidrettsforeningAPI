using GokstadFriidrettsforeningAPI.Models;
namespace GokstadFriidrettsforeningAPI.ModelResponses;

public class MemberRegistration
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public char Gender { get; set; }
    public Address? Address { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
    public string Password { get; set; } = string.Empty;
}