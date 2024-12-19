using GokstadFriidrettsforeningAPI.ModelResponses;
namespace GokstadFriidrettsforeningAPI.Features.Services.Interfaces;

public interface IRaceService : IService<RaceResponse>
{
    Task<RaceResponse?> RegisterAsync(RaceResponse regResponse);
    Task<IEnumerable<RaceResponse>> FindAsync(RacesQuery searchQuery);
}