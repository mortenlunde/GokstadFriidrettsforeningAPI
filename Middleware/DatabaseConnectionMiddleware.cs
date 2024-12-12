using GokstadFriidrettsforeningAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MySqlConnector;

namespace GokstadFriidrettsforeningAPI.Middleware;

public class DatabaseConnectionMiddleware(GaaDbContext dbContext, ILogger<DatabaseConnectionMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            logger.LogInformation("Starter tilkobling til database");
            await dbContext.Database.CanConnectAsync();
        }
        catch (MySqlException)
        {
            logger.LogError("Tilkobling til database feilet");
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "En feil oppsto i forsøk på å oppdatere til database");
        }
        catch (RetryLimitExceededException ex)
        {
            logger.LogError(ex, "Flere forsøk på å koble til database feilet");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "En uventet feil oppsto til database");
        }

        await next(context);
    }
}