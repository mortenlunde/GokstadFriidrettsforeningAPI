using GokstadFriidrettsforeningAPI.Models;

namespace GokstadFriidrettsforeningAPI.Features.Services.Interfaces;

public interface IRegistrationService : IService<Registration>
{
    Task<Registration?> RegisterAsync(int memberId, int raceId);
    Task<Registration?> DeleteRegistrationAsync(int memberId, int raceId);
}