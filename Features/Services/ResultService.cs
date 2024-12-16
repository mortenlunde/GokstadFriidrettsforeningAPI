using System.Linq.Expressions;
using GokstadFriidrettsforeningAPI.Features.Repositories;
using GokstadFriidrettsforeningAPI.Features.Repositories.Interfaces;
using GokstadFriidrettsforeningAPI.Features.Services.Interfaces;
using GokstadFriidrettsforeningAPI.Mappers;
using GokstadFriidrettsforeningAPI.Middleware;
using GokstadFriidrettsforeningAPI.ModelResponses;
using GokstadFriidrettsforeningAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GokstadFriidrettsforeningAPI.Features.Services;

public class ResultService(ILogger<RaceService> logger,
    IResultRepository resultRepository,
    IRaceRepository raceRepository,
    IMapper<Result, ResultResponse> mapper,
    IUserContextService httpContextAccessor) : IResultService
{
    public async Task<IEnumerable<ResultResponse>> GetPagedAsync(int pageNumber, int pageSize)
    {
        try
        {
            IEnumerable<Result> results = await resultRepository.GetPagedAsync(pageNumber, pageSize);
            return results.Select(mapper.MapToResponse).ToList();
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Ugyldige søkeparametre.");
            throw;
        }
        catch (UnauthorisedOperation ex)
        {
            logger.LogWarning(ex, "Du er ikke autorisert.");
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "En ukjent feil oppstro i henting av medlemmer");
            throw new Exception("En ukjent feil oppstro i henting av medlemmer.", ex);
        }
    }

    public async Task<ResultResponse> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<ResultResponse> DeleteByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<Result?> RegisterAsync(int memberId, int raceId, string time)
    {
        logger.LogInformation("Forsøker å registrere resultat for medlem {MemberId} til løp {RaceId}", memberId, raceId);

        var loggedInMemberId = httpContextAccessor.GetMemberId();
        if (loggedInMemberId == null)
        {
            logger.LogWarning("Ugyldig forespørsel: Mangler innlogget bruker.");
            throw new UnauthorizedAccessException("Innlogget bruker ikke funnet.");
        }

        if (loggedInMemberId != memberId)
        {
            logger.LogWarning("Medlem {MemberId} prøvde å registrere resultat for en annen medlemskonto {TargetMemberId}.", loggedInMemberId, memberId);
            throw new UnauthorizedAccessException("Du har ikke tilgang til denne medlemskontoen.");
        }

        var requestedRace = await raceRepository.GetByIdAsync(raceId);
        if (requestedRace == null)
        {
            logger.LogWarning("Løp med id {RaceId} eksisterer ikke.", raceId);
            return null;
        }

        var existingRegistration = await resultRepository.GetResultsByIdAsync(memberId, raceId);
        if (existingRegistration != null)
        {
            logger.LogWarning("Medlem {MemberId} har allerede registrert resultat til løp {RaceId}.", memberId, raceId);
            throw new InvalidOperationException("Medlemmet har allerede registrert resultat for dette løpet.");
        }

        var result = new Result()
        {
            MemberId = memberId,
            RaceId = raceId,
            Time = TimeSpan.Parse(time)
        };

        try
        {
            await resultRepository.AddAsync(result);
            logger.LogInformation("Nytt resultat registrert: Medlem {MemberId} til løp {RaceId}", memberId, raceId);
            return result;
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Feil for database ved registrering av resultat {MemberId} til løp {RaceId}", memberId, raceId);
            throw new Exception("En feil oppsto ved registrering av resultat.", ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ukjent feil for database ved registrering av resultat {MemberId} til løp {RaceId}", memberId, raceId);
            throw;
        }
    }

    public async Task<IEnumerable<ResultResponse>> FindAsync(ResultQuery searchQuery)
    {
        Expression<Func<Result, bool>> predicate = member =>
            (string.IsNullOrEmpty(searchQuery.MemberId.ToString()) || member.MemberId.ToString().Contains(searchQuery.MemberId.ToString())) && 
            (string.IsNullOrEmpty(searchQuery.RaceId.ToString()) || member.RaceId.ToString().Contains(searchQuery.RaceId.ToString())) &&
            (string.IsNullOrEmpty(searchQuery.Time.ToString()) || member.Time.ToString().Contains(searchQuery.Time.ToString()));

        IEnumerable<Result> results = await resultRepository.FindAsync(predicate);
        return results.Select(mapper.MapToResponse).ToList();
    }

    public async Task<Result?> DeleteResultAsync(int memberId, int raceId)
    {
        throw new NotImplementedException();
    }
}