namespace GokstadFriidrettsforeningAPI.Models;

public class Result
{
    public TimeSpan Time {get; set;}
    public int MemberId {get; set;}
    public int RaceId {get; set;}
    
    public virtual Member? Member { get; set; }
    public virtual Race? Race { get; set; }
}