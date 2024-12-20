using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MySqlConnector;
namespace GokstadFriidrettsforeningAPI.Middleware;

public class ExceptionHandling(ILogger<ExceptionHandling> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "En ukjent feil har oppstått.");
        httpContext.Response.ContentType = "application/json";

        int statusCode = exception switch
        {
            EmailAlreadyExistsException => StatusCodes.Status409Conflict,
            WrongUsernameOrPasswordException => StatusCodes.Status401Unauthorized,
            UnauthorisedOperation => StatusCodes.Status401Unauthorized,
            MySqlException => StatusCodes.Status503ServiceUnavailable,
            RetryLimitExceededException => StatusCodes.Status503ServiceUnavailable,
            DbUpdateException => StatusCodes.Status503ServiceUnavailable,
            NotFoundException => StatusCodes.Status404NotFound,
            DatabaseUnavailableException => StatusCodes.Status503ServiceUnavailable,
            _ => StatusCodes.Status500InternalServerError
        };

        string? statusDescription = Enum.IsDefined(typeof(HttpStatusCode), statusCode)
            ? Enum.GetName(typeof(HttpStatusCode), statusCode)
            : "Ukjent statuskode";
        
        string detailMessage = exception switch
        {
            MySqlException => "Forsøk på å koble til databasen feilet. Vennligst prøv igjen senere.",
            DatabaseUnavailableException dbEx => dbEx.Message,
            _ => exception.Message
        };

        ProblemDetails problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = statusDescription,
            Detail = detailMessage
        };

        var options = new JsonSerializerOptions { WriteIndented = true };
        await httpContext.Response.WriteAsJsonAsync(problemDetails, options, cancellationToken);
        return true;
    }
}

// Egne feilhåndteringer
public class EmailAlreadyExistsException(string email) 
    : Exception($"Epost '{email}' er allerede tatt i bruk av et annet medlem.");

public class WrongUsernameOrPasswordException() 
    : Exception("Brukernavn eller passord er feil.") { }

public class UnauthorisedOperation()
    : Exception("Uautorisert operasjon forsøkt. Du har ikke tilgang til dette objektet.") { }

public class NotFoundException()
    : Exception("Dette objektet finnes ikke.") { }
    
public class DatabaseUnavailableException(string message) 
    : Exception(message);