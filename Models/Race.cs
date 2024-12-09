namespace GokstadFriidrettsforeningAPI.Models;

public class Race
{
    public int RaceId { get; set; }
    public string RaceName { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public int Distance { get; set; }
    public int Laps { get; set; }

    public virtual ICollection<Result> Results { get; set; } = new HashSet<Result>();
}