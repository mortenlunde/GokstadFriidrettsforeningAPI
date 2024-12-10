using GokstadFriidrettsforeningAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MySqlConnector;

namespace GokstadFriidrettsforeningAPI.Middleware;

public class DatabaseConnectionMiddleware(GaaDbContext Context, ILogger<DatabaseConnectionMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            logger.LogInformation("Beginning to connect to database");
            await Context.Database.CanConnectAsync();
        }
        catch (MySqlException)
        {
            logger.LogError("Failed to connect to the database.");
            throw new Exception();
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "An error occurred during database update.");
            throw new Exception();
        }
        catch (RetryLimitExceededException ex)
        {
            logger.LogError(ex, "An error occurred during database connection.");
            throw new Exception();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred while connecting to the database.");
            throw new Exception();
        }

        await next(context);
    }
}