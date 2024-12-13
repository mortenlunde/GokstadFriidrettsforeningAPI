using System.Linq.Expressions;
using GokstadFriidrettsforeningAPI.Data;
using GokstadFriidrettsforeningAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GokstadFriidrettsforeningAPI.Features.Repositories;

public class RaceRepositry(ILogger<MemberRepository> logger, GaaDbContext context) : IRaceRepository
{
    public async Task<IEnumerable<Race>> GetPagedAsync(int pageNumber, int pageSize)
    {
        try
        {
            int skip = pageSize * (pageNumber - 1);
            
            var races = await context.Races
                .OrderBy(m => m.RaceId)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();
            
            return races;
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Feil for database ved henting av løp for side {PageNumber} med størrelse {PageSize}.", pageNumber, pageSize);
            throw new Exception("Feil for database ved henting av løp.", ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ukjent feil for database ved henting av løp.");
            throw new Exception("Ukjent feil for database ved henting av løp.\"", ex);
        }
    }

    public async Task<Race?> AddAsync(Race entity)
    {
        try
        {
            await context.Races.AddAsync(entity);
            await context.SaveChangesAsync();
            logger.LogInformation("Løp registrert: {name}", entity.RaceName);
            
            return entity;
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Feil for database ved registrering av løp {name}.", entity.RaceName);
            throw new Exception("Feil for database ved registrering av løp.", ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ukjent feil for database ved registrering av løp {name}.", entity.RaceName);
            throw new Exception("Ukjent feil for database ved registrering av løp", ex);
        }
    }

    public async Task<IEnumerable<Race>> FindAsync(Expression<Func<Race, bool>> predicate)
    {
        try
        {
            var races = await context.Races.Where(predicate).ToListAsync();
            logger.LogInformation("{Count} løp matchet søket.", races.Count);
            return races;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ukjent feil for database ved filtrering av løp");
            throw new Exception("Ukjent feil for database ved filtrering av løp.", ex);
        }
    }

    public async Task<Race?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<Race?> DeleteByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<Race?> UpdateByIdAsync(int id, Race entity)
    {
        throw new NotImplementedException();
    }
}