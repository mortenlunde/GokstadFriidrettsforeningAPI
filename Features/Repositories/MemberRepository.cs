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
        
        logger.LogInformation($"GetPagedAsync starting at {skip} of {pageSize}.");
        return await members;
    }
}