using System.ComponentModel.DataAnnotations;
namespace GokstadFriidrettsforeningAPI.Models;

public class Registration
{
    [Required]
    public int MemberId { get; set; }
    
    [Required]
    public int RaceId { get; set; }
    
    [Required]
    public DateTime RegistrationDate { get; set; }
    
    public virtual Member? Member { get; set; }
    public virtual Race? Race { get; set; }
}