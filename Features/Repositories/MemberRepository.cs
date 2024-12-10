using System.Linq.Expressions;
using GokstadFriidrettsforeningAPI.Data;
using GokstadFriidrettsforeningAPI.Models;
using Microsoft.EntityFrameworkCore;
namespace GokstadFriidrettsforeningAPI.Features.Repositories;

public class MemberRepository(ILogger<Member> logger, GaaDbContext context) : IMemberRepository
{
    public async Task<IEnumerable<Member>> GetPagedAsync(int pageNumber, int pageSize)
    {
        int skip = pageSize * (pageNumber - 1);

        Task<List<Member>> members = context.Members
            .OrderBy(m => m.MemberId)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync();
    
        logger.LogInformation($"Laster side {skip} med {pageSize} resultater.");
        return await members;
        
    }

    public async Task<Member?> AddAsync(Member entity)
    {
        if (await context.Members.AnyAsync(m => m.Email == entity.Email))
            return null;
        
        await context.Members.AddAsync(entity);
        await context.SaveChangesAsync();
        logger.LogInformation($"Medlem lagt til: {entity.FirstName} {entity.LastName}");
        
        return entity;
    }

    public async Task<IEnumerable<Member>> FindAsync(Expression<Func<Member, bool>> predicate)
    {
        return await context.Members.Where(predicate).ToListAsync();
    }
}