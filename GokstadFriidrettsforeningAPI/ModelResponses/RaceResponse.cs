namespace GokstadFriidrettsforeningAPI.ModelResponses;

public class RaceResponse
{
    public string RaceName { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public int Distance { get; set; }
    public int Laps { get; set; }
}