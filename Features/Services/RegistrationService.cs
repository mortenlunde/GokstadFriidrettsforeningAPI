using GokstadFriidrettsforeningAPI.Features.Repositories;
using GokstadFriidrettsforeningAPI.Features.Services.Interfaces;
using GokstadFriidrettsforeningAPI.Models;

namespace GokstadFriidrettsforeningAPI.Features.Services;

public class RegistrationService(ILogger<RaceService> logger,
    IRegistrationRepository regRepository,
    IUserContextService httpContextAccessor) : IRegistrationService
{
    public async Task<IEnumerable<Registration>> GetPagedAsync(int pageNumber, int pageSize)
    {
        throw new NotImplementedException();
    }

    public async Task<Registration> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<Registration> DeleteByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<Registration?> RegisterAsync(int memberId, int raceId)
    {
        logger.LogInformation("Forsøker å registrere medlem {MemberId} til løp {RaceId}", memberId, raceId);
        
        var loggedInMemberId = httpContextAccessor.GetMemberId();

        if (loggedInMemberId == null)
        {
            logger.LogWarning("Ugyldig forespørsel: Mangler innlogget bruker.");
            throw new UnauthorizedAccessException("Innlogget bruker ikke funnet.");
        }
        
        if (loggedInMemberId != memberId)
        {
            logger.LogWarning("Medlem: {MemberId} prøvde å legge til aktivitet for en annen medlemskonto: {TargetMemberId}.", loggedInMemberId, memberId);
            throw new UnauthorizedAccessException("Du har ikke tilgang til å endre denne medlemskontoen.");
        }
        
        var registration = new Registration
        {
            MemberId = memberId,
            RaceId = raceId,
            RegistrationDate = DateTime.UtcNow
        };

        var response = await regRepository.AddAsync(registration);

        if (response == null)
        {
            logger.LogWarning("Registrering mislyktes: Member {MemberId} til løp {RaceId}", memberId, raceId);
            return null;
        }

        logger.LogInformation("Ny aktivitet registrert: Member {MemberId} til løp {RaceId}", memberId, raceId);
        return response;
    }

}