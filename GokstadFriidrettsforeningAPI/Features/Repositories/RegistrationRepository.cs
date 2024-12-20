using System.Linq.Expressions;
using GokstadFriidrettsforeningAPI.Data;
using GokstadFriidrettsforeningAPI.Features.Repositories.Interfaces;
using GokstadFriidrettsforeningAPI.Middleware;
using GokstadFriidrettsforeningAPI.Models;
using Microsoft.EntityFrameworkCore;
using Exception = System.Exception;
namespace GokstadFriidrettsforeningAPI.Features.Repositories;
/// <summary>
/// Repository for håndtering av CRUD-operasjoner i databasen.
/// Håndterer også filtrering, paginering, og logging av operasjoner.
/// </summary>
public class RegistrationRepository(ILogger<MemberRepository> logger, GaaDbContext context) : IRegistrationRepository
{
    public async Task<IEnumerable<Registration>> GetPagedAsync(int pageNumber, int pageSize)
    {
        throw new NotImplementedException();
    }

    public async Task<Registration?> AddAsync(Registration entity)
    {
        try
        {
            await context.Registrations.AddAsync(entity);
            await context.SaveChangesAsync();
            logger.LogInformation("Aktivitet registrert: {memberid} til løp {raceid}", entity.MemberId, entity.RaceId);

            return entity;
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Feil for database ved registrering av aktivitet {memberid} til løp {raceid}",
                entity.MemberId, entity.RaceId);
            throw new Exception("Feil for database ved registrering av aktivitet.", ex);
        }
        catch (KeyNotFoundException)
        {
            logger.LogError("Feil for database ved registrering av aktivitet. Løpet med id {id} eksisterer ikke.", entity.RaceId);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ukjent feil for database ved registrering av aktivitet {memberid} til løp {raceid}", entity.MemberId, entity.RaceId);
            throw new Exception("Ukjent feil for database ved registrering av aktivitet", ex);
        }
    }

    public async Task<IEnumerable<Registration>> FindAsync(Expression<Func<Registration, bool>> predicate)
    {
        throw new NotImplementedException();
    }

    public async Task<Registration?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<Registration?> DeleteByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<Registration?> DeleteRegistrationByIdAsync(int memberId, int raceId)
    {
        try
        {
            Registration? registration = await context.Registrations.FindAsync(memberId, raceId);
            if (registration == null) return null;
            await context.Registrations.Where(m => m.MemberId == memberId && m.RaceId == raceId).ExecuteDeleteAsync();
            await context.SaveChangesAsync();
            return registration;
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

    public async Task<Registration?> UpdateByIdAsync(int id, Registration entity)
    {
        throw new NotImplementedException();
    }

    public async Task<Registration?> GetRegistrationByIdAsync(int memberId, int activityId)
    {
        return await context.Registrations.FirstOrDefaultAsync(m => m.MemberId == memberId && m.RaceId == activityId);
    }
}