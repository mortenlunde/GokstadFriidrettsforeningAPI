using GokstadFriidrettsforeningAPI.ModelResponses;
using GokstadFriidrettsforeningAPI.Models;

namespace GokstadFriidrettsforeningAPI.Features.Services.Interfaces;

public interface IResultService : IService<ResultResponse>
{
    Task<Result?> RegisterAsync(int memberId, int raceId, string time);
    Task<IEnumerable<ResultResponse>> FindAsync(ResultQuery searchQuery);
    Task<Result?> DeleteResultAsync(int memberId, int raceId);
}