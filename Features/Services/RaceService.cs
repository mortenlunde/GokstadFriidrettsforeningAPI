using System.Linq.Expressions;
using GokstadFriidrettsforeningAPI.Features.Repositories;
using GokstadFriidrettsforeningAPI.Features.Services.Interfaces;
using GokstadFriidrettsforeningAPI.Mappers;
using GokstadFriidrettsforeningAPI.Middleware;
using GokstadFriidrettsforeningAPI.ModelResponses;
using GokstadFriidrettsforeningAPI.Models;

namespace GokstadFriidrettsforeningAPI.Features.Services;

public class RaceService(
    ILogger<RaceService> logger,
    IMapper<Race, RaceResponse> mapper,
    IRaceRepository repository,
    IUserContextService httpContextAccessor) : IRaceService
{
    public async Task<IEnumerable<RaceResponse>> GetPagedAsync(int pageNumber, int pageSize)
    {
        try
        {
            IEnumerable<Race> races = await repository.GetPagedAsync(pageNumber, pageSize);
            return races.Select(mapper.MapToResponse).ToList();
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

    public async Task<RaceResponse> GetByIdAsync(int id)
    {
        Race? race = await repository.GetByIdAsync(id);
        if (race == null)
            throw new NotFoundException();
        return mapper.MapToResponse(race);
    }

    public async Task<IEnumerable<RaceResponse>> FindAsync(RacesQuery searchParameters)
    {
        Expression<Func<Race, bool>> predicate = race =>
            (string.IsNullOrEmpty(searchParameters.RaceName) || race.RaceName.Contains(searchParameters.RaceName)) &&
            (!searchParameters.Date.HasValue || race.Date == searchParameters.Date.Value) &&
            (!searchParameters.Distance.HasValue || race.Distance == searchParameters.Distance.Value) &&
            (!searchParameters.Laps.HasValue || race.Laps == searchParameters.Laps.Value);

        IEnumerable<Race> members = await repository.FindAsync(predicate);
        return members.Select(mapper.MapToResponse).ToList();
    }

    public async Task<RaceResponse> DeleteByIdAsync(int id)
    {
        logger.LogInformation("Forsøker å slette løp med id {RaceId}.", id);

        var loggedInMemberId = httpContextAccessor.GetMemberId();
        if (loggedInMemberId == null)
        {
            logger.LogWarning("Ugyldig forespørsel: Mangler innlogget bruker.");
            throw new UnauthorizedAccessException("Innlogget bruker ikke funnet.");
        }
        
        if (loggedInMemberId != id)
        {
            logger.LogWarning("Medlem {MemberId} prøvde å slette et løp opprettet av medlem {TargetMemberId}.", loggedInMemberId, id);
            throw new UnauthorizedAccessException("Du har ikke tilgang til denne medlemskontoen.");
        }

        var raceToDelete = await repository.GetByIdAsync(id);
        if (raceToDelete == null)
        {
            logger.LogWarning("Løp med id {id} eksisterer ikke.", id);
            throw new KeyNotFoundException($"Løp med id {id} eksisterer ikke.");
        }

        await repository.DeleteByIdAsync(id);
        logger.LogInformation("Løp {RaceId} ble slettet.", id);

        return mapper.MapToResponse(raceToDelete);
    }

    public async Task<RaceResponse?> RegisterAsync(RaceResponse regResponse)
    {
        try
        {
            logger.LogInformation("Forsøker å registrere løp {name}", regResponse.RaceName);
            Race race = mapper.MapToModel(regResponse);
            
            
            Race? raceResponse = await repository.AddAsync(race);
            if (raceResponse is null)
            {
                logger.LogError("Registreting av løp {name} feilet", regResponse.RaceName);
                return null;
            }

            logger.LogInformation("Nytt løp regisertert: {name}", regResponse.RaceName);
            return mapper.MapToResponse(raceResponse);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "En ukjent feil oppsto ved registrering");
            throw new Exception("En ukjent feil oppsto ved registrering.", ex);
        }
    }
}