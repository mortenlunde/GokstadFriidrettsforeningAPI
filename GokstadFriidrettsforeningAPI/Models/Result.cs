using System.Text.Json.Serialization;

namespace GokstadFriidrettsforeningAPI.Models;

public class Result
{
    public TimeSpan Time {get; set;}
    public int MemberId {get; set;}
    public int RaceId {get; set;}
    
    [JsonIgnore]
    public virtual Member? Member { get; set; }
    
    [JsonIgnore]
    public virtual Race? Race { get; set; }
}