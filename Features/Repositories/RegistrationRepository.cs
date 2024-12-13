using System.Linq.Expressions;
using GokstadFriidrettsforeningAPI.Data;
using GokstadFriidrettsforeningAPI.Middleware;
using GokstadFriidrettsforeningAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GokstadFriidrettsforeningAPI.Features.Repositories;

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
            logger.LogError(ex, "Feil for database ved registrering av aktivitet {memberid} til løp {raceid}", entity.MemberId, entity.RaceId);
            throw new Exception("Feil for database ved registrering av aktivitet.", ex);
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

    public async Task<Registration?> UpdateByIdAsync(int id, Registration entity)
    {
        throw new NotImplementedException();
    }
}