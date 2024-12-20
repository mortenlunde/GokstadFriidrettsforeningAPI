using GokstadFriidrettsforeningAPI.Models;
namespace GokstadFriidrettsforeningAPI.Features.Repositories.Interfaces;

public interface IResultRepository : IRepositry<Result>
{
    Task<Result?> DeleteResultByIdAsync(int memberId, int activityId);
    Task<Result?> GetResultsByIdAsync(int memberId, int activityId);
}