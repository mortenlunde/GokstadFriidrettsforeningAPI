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