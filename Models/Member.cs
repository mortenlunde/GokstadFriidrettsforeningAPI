using Microsoft.EntityFrameworkCore;
namespace GokstadFriidrettsforeningAPI.Models;

public class Member
{
    public int MemberId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public char Gender { get; set; }
    public Address? Address { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
    public string HashedPassword { get; set; } = string.Empty;
    
    public virtual ICollection<Result> Results { get; set; } = new HashSet<Result>();
}

[Owned]
public class Address
{
    public string Street { get; set; } = string.Empty;
    public int PostalCode { get; set; }
    public string City { get; set; } = string.Empty;
}