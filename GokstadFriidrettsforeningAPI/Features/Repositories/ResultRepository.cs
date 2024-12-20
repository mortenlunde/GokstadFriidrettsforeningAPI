using System.Linq.Expressions;
using GokstadFriidrettsforeningAPI.Data;
using GokstadFriidrettsforeningAPI.Features.Repositories.Interfaces;
using GokstadFriidrettsforeningAPI.Middleware;
using GokstadFriidrettsforeningAPI.Models;
using Microsoft.EntityFrameworkCore;
using Exception = System.Exception;

namespace GokstadFriidrettsforeningAPI.Features.Repositories;
/// <summary>
/// Repository for handling CRUD-operasjoner på medlemmer i databasen.
/// Håndterer også filtrering, paginering, og logging av operasjoner.
/// </summary>
public class ResultRepository(ILogger<MemberRepository> logger, GaaDbContext context) : IResultRepository
{
    public async Task<IEnumerable<Result>> GetPagedAsync(int pageNumber, int pageSize)
    {
        try
        {
            int skip = pageSize * (pageNumber - 1);
            
            var results = await context.Results
                .OrderBy(m => m.Time)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();
            
            return results;
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Feil for database ved henting av medlemmer for side {PageNumber} med størrelse {PageSize}.", pageNumber, pageSize);
            throw new Exception("Feil for database ved henting av medlemmer.", ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ukjent feil for database ved henting av medlemmer.");
            throw new Exception("Ukjent feil for database ved henting av medlemmer.\"", ex);
        }
    }

    public async Task<Result?> AddAsync(Result entity)
    {
        try
        {
            await context.Results.AddAsync(entity);
            await context.SaveChangesAsync();
            logger.LogInformation("Resultat registrert: {memberid} til løp {raceid}", entity.MemberId, entity.RaceId);

            return entity;
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Feil for database ved registrering av resultat {memberid} til løp {raceid}",
                entity.MemberId, entity.RaceId);
            throw new Exception("Feil for database ved registrering av resultat.", ex);
        }
        catch (KeyNotFoundException)
        {
            logger.LogError("Feil for database ved registrering av resultat. Løpet med id {id} eksisterer ikke.", entity.RaceId);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ukjent feil for database ved registrering av resultat {memberid} til løp {raceid}", entity.MemberId, entity.RaceId);
            throw new Exception("Ukjent feil for database ved registrering av resultat", ex);
        }
    }

    public async Task<IEnumerable<Result>> FindAsync(Expression<Func<Result, bool>> predicate)
    {
        try
        {
            var results = await context.Results.Where(predicate).OrderBy(r => r.Time).ToListAsync();
            logger.LogInformation("{Count} resultater matchet søket.", results.Count);
            return results;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ukjent feil for database ved filtrering av resultater");
            throw new Exception("Ukjent feil for database ved filtrering av resultater.", ex);
        }
    }

    public async Task<Result?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<Result?> DeleteByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<Result?> UpdateByIdAsync(int id, Result entity)
    {
        throw new NotImplementedException();
    }

    public async Task<Result?> GetResultByIdAsync(int memberId, int activityId)
    {
        return await context.Results.FirstOrDefaultAsync(m => m.MemberId == memberId && m.RaceId == activityId);
    }

    public async Task<Result?> DeleteResultByIdAsync(int memberId, int raceId)
    {
        try
        {
            Result? result = await context.Results.FindAsync(memberId, raceId);
            if (result == null) return null;
            await context.Results.Where(m => m.MemberId == memberId && m.RaceId == raceId).ExecuteDeleteAsync();
            await context.SaveChangesAsync();
            return result;
        }
        catch (UnauthorisedOperation e)
        {
            Console.WriteLine(e);
            throw;
        }
        catch (KeyNotFoundException e)
        {
            Console.WriteLine(e);
            throw;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<Result?> GetResultsByIdAsync(int memberId, int activityId)
    {
        return await context.Results.FirstOrDefaultAsync(m => m.MemberId == memberId && m.RaceId == activityId);
    }
}