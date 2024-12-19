using System.ComponentModel.DataAnnotations;

namespace GokstadFriidrettsforeningAPI.Features.Services;

public class MemberQuery
{
    public string Firstname { get; init; } = null!;
    public string Lastname { get; init; } = null!;
    public string Email { get; init; } = null!;
}

public class RacesQuery
{
    public string? RaceName { get; init; }
    public DateOnly? Date { get; set; }
    public int? Distance { get; set; }
    public int? Laps { get; set; }
}

public class ResultQuery
{
    public int? MemberId { get; set; }
    
    [Required]
    public int? RaceId { get; set; }
    public TimeSpan? Time { get; set; }
}