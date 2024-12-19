using GokstadFriidrettsforeningAPI.Features.Repositories;
using GokstadFriidrettsforeningAPI.Features.Services.Interfaces;
using GokstadFriidrettsforeningAPI.Models;
using Microsoft.EntityFrameworkCore;
using InvalidOperationException = System.InvalidOperationException;
using UnauthorizedAccessException = System.UnauthorizedAccessException;

namespace GokstadFriidrettsforeningAPI.Features.Services;

public class RegistrationService(ILogger<RaceService> logger,
    IRegistrationRepository regRepository,
    IRaceRepository raceRepository,
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

    int? loggedInMemberId = httpContextAccessor.GetMemberId();
    if (loggedInMemberId == null)
    {
        logger.LogWarning("Ugyldig forespørsel: Mangler innlogget bruker.");
        throw new UnauthorizedAccessException("Innlogget bruker ikke funnet.");
    }

    if (loggedInMemberId != memberId)
    {
        logger.LogWarning("Medlem {MemberId} prøvde å manipulere en annen medlemskonto {TargetMemberId}.", loggedInMemberId, memberId);
        throw new UnauthorizedAccessException("Du har ikke tilgang til denne medlemskontoen.");
    }

    var requestedRace = await raceRepository.GetByIdAsync(raceId);
    if (requestedRace == null)
    {
        logger.LogWarning("Løp med id {RaceId} eksisterer ikke.", raceId);
        return null;
    }

    var existingRegistration = await regRepository.GetRegistrationByIdAsync(memberId, raceId);
    if (existingRegistration != null)
    {
        logger.LogWarning("Medlem {MemberId} er allerede registrert til løp {RaceId}.", memberId, raceId);
        throw new InvalidOperationException("Medlemmet er allerede registrert for dette løpet.");
    }

    var registration = new Registration
    {
        MemberId = memberId,
        RaceId = raceId,
        RegistrationDate = DateTime.UtcNow
    };

    try
    {
        await regRepository.AddAsync(registration);
        logger.LogInformation("Ny aktivitet registrert: Medlem {MemberId} til løp {RaceId}", memberId, raceId);
        return registration;
    }
    catch (DbUpdateException ex)
    {
        logger.LogError(ex, "Feil for database ved registrering av aktivitet {MemberId} til løp {RaceId}", memberId, raceId);
        throw new Exception("En feil oppsto ved registrering av aktivitet.", ex);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Ukjent feil for database ved registrering av aktivitet {MemberId} til løp {RaceId}", memberId, raceId);
        throw;
    }
}

    public async Task<Registration?> DeleteRegistrationAsync(int memberId, int raceId)
    {
        logger.LogInformation("Forsøker å slette påmelding for medlem {MemberId} og løp {RaceId}.", memberId, raceId);

        int? loggedInMemberId = httpContextAccessor.GetMemberId();
        if (loggedInMemberId == null)
        {
            logger.LogWarning("Ugyldig forespørsel: Mangler innlogget bruker.");
            throw new UnauthorizedAccessException("Innlogget bruker ikke funnet.");
        }

        if (loggedInMemberId != memberId)
        {
            logger.LogWarning("Medlem {MemberId} prøvde å manipulere en annen medlemskonto {TargetMemberId}.", loggedInMemberId, memberId);
            throw new UnauthorizedAccessException("Du har ikke tilgang til denne medlemskontoen.");
        }

        var requestedRace = await raceRepository.GetByIdAsync(raceId);
        if (requestedRace == null)
        {
            logger.LogWarning("Løp med id {id} eksisterer ikke.", raceId);
            throw new KeyNotFoundException($"Løp med id {raceId} eksisterer ikke.");
        }

        var registrationToDelete = await regRepository.GetRegistrationByIdAsync(memberId, raceId);
        if (registrationToDelete == null)
        {
            logger.LogWarning("Ingen registrering funnet for medlem {MemberId} og løp {RaceId}.", memberId, raceId);
            throw new KeyNotFoundException("Registreringen ble ikke funnet.");
        }

        await regRepository.DeleteRegistrationByIdAsync(memberId, raceId);
        logger.LogInformation("Aktivitet for medlem {MemberId} og løp {RaceId} ble slettet.", memberId, raceId);

        return registrationToDelete;
    }
}