using GokstadFriidrettsforeningAPI.ModelResponses;
using GokstadFriidrettsforeningAPI.Models;

namespace GokstadFriidrettsforeningAPI.Mappers;

public class ResultMapper : IMapper<Result, ResultResponse>
{
    public ResultResponse MapToResponse(Result model)
    {
        return new ResultResponse()
        {
            MemberId = model.MemberId,
            RaceId = model.RaceId,
            Time = model.Time,
        };
    }

    public Result MapToModel(ResultResponse response)
    {
        return new Result()
        {
            MemberId = response.MemberId,
            RaceId = response.RaceId,
            Time = response.Time,
        };
    }
}