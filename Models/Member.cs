using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GokstadFriidrettsforeningAPI.Models;

public class Member
{
    [Key]
    public int MemberId { get; set; }

    [Required]
    [StringLength(30, MinimumLength = 3)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(1)]
    [RegularExpression("[MFU]", ErrorMessage = "Gender must be 'M', 'F', or 'U'.")]
    public char Gender { get; set; }

    [Required]
    public DateOnly DateOfBirth { get; set; }

    [Required]
    public DateTime Created { get; set; }

    [Required]
    public DateTime Updated { get; set; }

    [Required]
    public string HashedPassword { get; set; } = string.Empty;

    public Address? Address { get; set; }

    public virtual ICollection<Result> Results { get; set; } = new HashSet<Result>();
}

[ComplexType]
public class Address
{
    [Required]
    [StringLength(100)]
    public string Street { get; set; } = string.Empty;

    [Required]
    [Range(1000, 9999)]
    public int PostalCode { get; set; }

    [Required]
    [StringLength(50)]
    public string City { get; set; } = string.Empty;
}