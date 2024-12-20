using System.ComponentModel.DataAnnotations;
namespace GokstadFriidrettsforeningAPI.Models;

public class Race
{
    [Key]
    public int RaceId { get; set; }
    
    [Required, MaxLength(50)]
    public string RaceName { get; set; } = string.Empty;
    
    public DateOnly Date { get; set; }
    public int Distance { get; set; }
    public int Laps { get; set; }

    public virtual ICollection<Registration> Registrations { get; set; }
    public virtual ICollection<Result> Results { get; set; } = new HashSet<Result>();
}