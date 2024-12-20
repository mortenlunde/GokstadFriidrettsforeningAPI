using GokstadFriidrettsforeningAPI.Models;
namespace GokstadFriidrettsforeningAPI.Features.Repositories.Interfaces;

public interface IRegistrationRepository : IRepositry<Registration>
{
    Task<Registration?> GetRegistrationByIdAsync(int memberId, int activityId);
    Task<Registration?> DeleteRegistrationByIdAsync(int memberId, int activityId);
}