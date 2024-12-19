using System.ComponentModel.DataAnnotations;
namespace GokstadFriidrettsforeningAPI.Models;

public class Log
{
    [Key]
    public int? Id { get; set; }
    
    [StringLength(100)]
    public string? Timestamp { get; set; }
    
    [StringLength(15)]
    public string? Level { get; set; }
    
    [StringLength(65535)]
    public string? Message { get; set; }
    
    [StringLength(65535)]
    public string? Exception { get; set; }
}