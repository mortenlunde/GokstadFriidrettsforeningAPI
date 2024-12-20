using System.Linq.Expressions;
using GokstadFriidrettsforeningAPI.Features.Repositories.Interfaces;
using GokstadFriidrettsforeningAPI.Features.Services.Interfaces;
using GokstadFriidrettsforeningAPI.Mappers;
using GokstadFriidrettsforeningAPI.Middleware;
using GokstadFriidrettsforeningAPI.ModelResponses;
using GokstadFriidrettsforeningAPI.Models;
using UnauthorizedAccessException = System.UnauthorizedAccessException;
namespace GokstadFriidrettsforeningAPI.Features.Services;
/// <summary>
/// Service-laget håndterer forretningslogikken for medlemmer. 
/// Utfører validering, koordinering mellom repository og eventuelle andre tjenester.
/// </summary>

public class MemberService(
    ILogger<MemberService> logger,
    IMapper<Member, MemberResponse> mapper,
    IMemberRepository repository,
    IMapper<Member, MemberRegistration> regMapper,
    IUserContextService httpContextAccessor) : IMemberService
{
    public async Task<IEnumerable<MemberResponse>> GetPagedAsync(int pageNumber, int pageSize)
    {
        try
        {
            IEnumerable<Member> members = await repository.GetPagedAsync(pageNumber, pageSize);
            return members.Select(mapper.MapToResponse).ToList();
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

    public async Task<MemberResponse> GetByIdAsync(int id)
    {
        Member? member = await repository.GetByIdAsync(id);
        if (member == null)
            throw new NotFoundException();
        return mapper.MapToResponse(member);
    }

    public async Task<IEnumerable<MemberResponse>> FindAsync(MemberQuery searchQuery)
    {
        Expression<Func<Member, bool>> predicate = member =>
            (string.IsNullOrEmpty(searchQuery.Firstname) || member.FirstName.Contains(searchQuery.Firstname)) && 
            (string.IsNullOrEmpty(searchQuery.Lastname) || member.LastName.Contains(searchQuery.Lastname)) &&
            (string.IsNullOrEmpty(searchQuery.Email) || member.Email.Contains(searchQuery.Email));

        IEnumerable<Member> members = await repository.FindAsync(predicate);
        return members.Select(mapper.MapToResponse).ToList();
    }

    public async Task<MemberResponse> DeleteByIdAsync(int id)
    {
        var loggedInMemberId = httpContextAccessor.GetMemberId();

        if (loggedInMemberId != id)
        {
            logger.LogWarning("Medlem {MemberId} prøvde å slette en annen medlemskonto {TargetMemberId}.", loggedInMemberId, id);
            throw new UnauthorizedAccessException("Du har ikke tilgang til å slette denne medlemskontoen.");
        }

        var memberToDelete = await repository.GetByIdAsync(id);
        if (memberToDelete == null)
        {
            logger.LogWarning("Forsøkte å slette et medlem som ikke eksisterer med ID {MemberId}.", id);
            throw new KeyNotFoundException($"Medlem med ID {id} ble ikke funnet.");
        }

        await repository.DeleteByIdAsync(id);
        logger.LogInformation("Medlem {MemberId} slettet sin egen medlemskonto.", loggedInMemberId);

        return mapper.MapToResponse(memberToDelete);
    }

    public async Task<MemberResponse?> RegisterAsync(MemberRegistration regResponse)
    {
        try
        {
            logger.LogInformation("Forsøker å registrere bruker med epost {Email}", regResponse.Email);
            Member member = regMapper.MapToModel(regResponse);
            member.Created = DateTime.UtcNow;
            member.Updated = DateTime.UtcNow;
            member.HashedPassword = BCrypt.Net.BCrypt.HashPassword(regResponse.Password);
            
            Member? userResponse = await repository.AddAsync(member);
            if (userResponse is null)
            {
                logger.LogError("Registreting av medlem med epost {Email} feilet", regResponse.Email);
                return null;
            }

            logger.LogInformation("Nytt medlem regisertert: {Email}", regResponse.Email);
            return mapper.MapToResponse(userResponse);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "En ukjent feil oppsto ved registrering");
            throw new Exception("En ukjent feil oppsto ved registrering.", ex);
        }
    }

    public async Task<Member> AuthenticateUserAsync(string email, string password)
    {
        try
        {
            logger.LogInformation("Forsøker å autorisere med epost {Email}", email);

            Expression<Func<Member, bool>> expression = user => user.Email == email;
            Member? user = (await repository.FindAsync(expression)).FirstOrDefault();

            if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.HashedPassword))
            {
                logger.LogWarning("Ugyldig forsøk på innlogging med epost {Email}", email);
                throw new WrongUsernameOrPasswordException();
            }

            logger.LogInformation("Medlem logget vellykket inn med epost {Email}", email);
            return user;
        }
        catch (WrongUsernameOrPasswordException ex)
        {
            logger.LogWarning(ex, "Ugyldig forsøk på innlogging med epost {Email}", email);
            throw new UnauthorisedOperation();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "En ukjent feil ved innlogging har oppstått");
            throw new Exception("En ukjent feil ved innlogging har oppstått.", ex);
        }
    }

    public async Task<MemberResponse> UpdateMemberAsync(int id, MemberRegistration entity)
    {
        int? loggedInMemberId = httpContextAccessor.GetMemberId();

        if (loggedInMemberId == null)
        {
            logger.LogWarning("Ugyldig forespørsel: Mangler innlogget bruker.");
            throw new UnauthorizedAccessException("Innlogget bruker ikke funnet.");
        }
        
        if (loggedInMemberId != id)
        {
            logger.LogWarning("Medlem {MemberId} prøvde å endre en annen medlemskonto {TargetMemberId}.", loggedInMemberId, id);
            throw new UnauthorizedAccessException("Du har ikke tilgang til å endre denne medlemskontoen.");
        }

        var memberToUpdate = await repository.GetByIdAsync(id);
        if (memberToUpdate == null)
        {
            logger.LogWarning("Forsøkte å endre et medlem som ikke eksisterer med ID {MemberId}.", id);
            throw new KeyNotFoundException($"Medlem med ID {id} ble ikke funnet.");
        }

        await repository.UpdateByIdAsync(id, memberToUpdate);
        logger.LogInformation("Medlem {MemberId} endret sin egen medlemskonto.", loggedInMemberId);
        
        if (!string.IsNullOrWhiteSpace(entity.FirstName))
            memberToUpdate.FirstName = entity.FirstName;
        
        if (!string.IsNullOrWhiteSpace(entity.LastName))
            memberToUpdate.LastName = entity.LastName;
        
        if (!string.IsNullOrWhiteSpace(entity.Email))
            memberToUpdate.Email = entity.Email;
        
        if (!string.IsNullOrWhiteSpace(entity.Address?.Street))
            memberToUpdate.Address!.Street = entity.Address.Street;
        
        if (!string.IsNullOrWhiteSpace(entity.Address?.City))
            memberToUpdate.Address!.City = entity.Address.City;
        
        if (!string.IsNullOrWhiteSpace(entity.Address?.PostalCode.ToString()))
            memberToUpdate.Address!.PostalCode = entity.Address.PostalCode;
        
        if (!string.IsNullOrWhiteSpace(entity.DateOfBirth.ToString()))
            memberToUpdate.DateOfBirth = entity.DateOfBirth;
        
        if (!string.IsNullOrWhiteSpace(entity.Gender.ToString()))
            memberToUpdate.Gender = entity.Gender;
        
        if (!string.IsNullOrWhiteSpace(entity.Password))
            memberToUpdate.HashedPassword = BCrypt.Net.BCrypt.HashPassword(entity.Password);
        
        memberToUpdate.Updated = DateTime.UtcNow;
        
        await repository.UpdateByIdAsync(id, memberToUpdate);

        return mapper.MapToResponse(memberToUpdate);
    }
}
