using System.ComponentModel.DataAnnotations;
namespace GokstadFriidrettsforeningAPI.Features.Services;
/// <summary>
/// Søkeparametre for de forskjellige modellene
/// </summary>
public class MemberQuery
{
    public string Firstname { get; init; } = null!;
    public string Lastname { get; init; } = null!;
    public string Email { get; init; } = null!;
}

public class RacesQuery
{
    public string? RaceName { get; init; } = null!;
    public DateOnly? Date { get; set; } = null!;
    public int? Distance { get; set; } = null!;
    public int? Laps { get; set; } = null!;
}

public class ResultQuery
{
    public int? MemberId { get; set; } = null!;
    
    [Required]
    public int? RaceId { get; set; } = null!;
    public TimeSpan? Time { get; set; } = null!;
}