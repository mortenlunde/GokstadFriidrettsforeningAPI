using System.Linq.Expressions;
using GokstadFriidrettsforeningAPI.Data;
using GokstadFriidrettsforeningAPI.Middleware;
using GokstadFriidrettsforeningAPI.Models;
using Microsoft.EntityFrameworkCore;
namespace GokstadFriidrettsforeningAPI.Features.Repositories;

public class MemberRepository(ILogger<MemberRepository> logger, GaaDbContext context) : IMemberRepository
{
    public async Task<IEnumerable<Member>> GetPagedAsync(int pageNumber, int pageSize)
    {
        try
        {
            int skip = pageSize * (pageNumber - 1);
            
            var members = await context.Members
                .OrderBy(m => m.MemberId)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();
            
            return members;
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

    public async Task<Member?> AddAsync(Member entity)
    {
        try
        {
            if (await context.Members.AnyAsync(m => m.Email == entity.Email))
            {
                logger.LogWarning("Medlem med epost {Email} finnes allerede", entity.Email);
                throw new EmailAlreadyExistsException("Et medlem med samme epost finnes allerede.");
            }

            await context.Members.AddAsync(entity);
            await context.SaveChangesAsync();
            logger.LogInformation("Medlem registrert: {FirstName} {LastName}, Epost: {Email}", entity.FirstName, entity.LastName, entity.Email);
            
            return entity;
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Feil for database ved registrering av medlem med epost {Email}.", entity.Email);
            throw new Exception("Feil for database ved registrering av medlem.", ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ukjent feil for database ved registrering av medlem med epost {Email}.", entity.Email);
            throw new Exception("Ukjent feil for database ved registrering av medlem", ex);
        }
    }

    public async Task<IEnumerable<Member>> FindAsync(Expression<Func<Member, bool>> predicate)
    {
        try
        {
            var members = await context.Members.Where(predicate).ToListAsync();
            logger.LogInformation("{Count} medlemmer matchet søket.", members.Count);
            return members;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ukjent feil for database ved filtrering av medlemmer");
            throw new Exception("Ukjent feil for database ved filtrering av medlemmer.", ex);
        }
    }

    public async Task<Member?> GetByIdAsync(int id)
    {
        return await context.Members.FirstOrDefaultAsync(m => m.MemberId == id);
    }

    public async Task<Member?> DeleteByIdAsync(int id)
    {
        try
        {
            Member? member = await context.Members.FindAsync(id);
            await context.Members.Where(m => m.MemberId == id).ExecuteDeleteAsync();
            await context.SaveChangesAsync();
            return member;
        }
        catch (UnauthorisedOperation e)
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

    public async Task<Member?> UpdateByIdAsync(int id, Member entity)
    {
        Member? member = await context.Members.FindAsync(id);
        if (member == null) return null;
        
        if (!string.IsNullOrWhiteSpace(entity.FirstName))
            member.FirstName = entity.FirstName;
        
        if (!string.IsNullOrWhiteSpace(entity.LastName))
            member.LastName = entity.LastName;
        
        if (!string.IsNullOrWhiteSpace(entity.Email))
            member.Email = entity.Email;
        
        if (!string.IsNullOrWhiteSpace(entity.Address?.Street))
            member.Address!.Street = entity.Address.Street;
        
        if (!string.IsNullOrWhiteSpace(entity.Address?.City))
            member.Address!.City = entity.Address.City;
        
        if (!string.IsNullOrWhiteSpace(entity.Address?.PostalCode.ToString()))
            member.Address!.PostalCode = entity.Address.PostalCode;
        
        if (!string.IsNullOrWhiteSpace(entity.DateOfBirth.ToString()))
            member.DateOfBirth = entity.DateOfBirth;
        
        if (!string.IsNullOrWhiteSpace(entity.Gender.ToString()))
            member.Gender = entity.Gender;
        
        if (!string.IsNullOrWhiteSpace(entity.HashedPassword))
            member.HashedPassword = entity.HashedPassword;
        
        member.Updated = DateTime.UtcNow;
        await context.SaveChangesAsync();
        return member;
    }
}
