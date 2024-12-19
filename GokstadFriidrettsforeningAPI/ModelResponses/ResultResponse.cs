namespace GokstadFriidrettsforeningAPI.ModelResponses;

public class ResultResponse
{
    public TimeSpan Time {get; set;}
    public int MemberId {get; set;}
    public int RaceId {get; set;}
}

public class ResultDeleteResponse
{
    public int MemberId {get; set;}
    public int RaceId {get; set;}
}