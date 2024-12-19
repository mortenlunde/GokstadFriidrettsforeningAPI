using GokstadFriidrettsforeningAPI.ModelResponses;
using GokstadFriidrettsforeningAPI.Models;

namespace GokstadFriidrettsforeningAPI.Mappers;

public class RaceMapper : IMapper<Race, RaceResponse>
{
    public RaceResponse MapToResponse(Race model)
    {
        return new RaceResponse
        {
            RaceName = model.RaceName,
            Date = model.Date,
            Distance = model.Distance,
            Laps = model.Laps,
        };
    }

    public Race MapToModel(RaceResponse response)
    {
        return new Race
        {
            RaceName = response.RaceName,
            Date = response.Date,
            Distance = response.Distance,
            Laps = response.Laps,
        };
    }
}