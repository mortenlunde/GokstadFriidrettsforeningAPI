using System.Reflection;
using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using GokstadFriidrettsforeningAPI.Data;
using GokstadFriidrettsforeningAPI.Features.Repositories;
using GokstadFriidrettsforeningAPI.Features.Repositories.Interfaces;
using GokstadFriidrettsforeningAPI.Features.Services;
using GokstadFriidrettsforeningAPI.Features.Services.Interfaces;
using GokstadFriidrettsforeningAPI.Mappers;
using GokstadFriidrettsforeningAPI.Middleware;
using GokstadFriidrettsforeningAPI.TokenHandling;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;
namespace GokstadFriidrettsforeningAPI.Extensions;

public static class ServiceCollectionExtension
{
    // For bruk av JWT i SwaggerUI
    public static void AddSwaggerJwtAuthentication(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...'"
            });
            c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
            {
                {
                    new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Reference = new Microsoft.OpenApi.Models.OpenApiReference
                        {
                            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    []
                }
            });
        });
    }
    
    // Oppsett/konfigurering av JWT som authentication i APIet
    public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection("JWT"));
        JwtOptions? jwtOptions = configuration.GetSection("JWT").Get<JwtOptions>();
        
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOptions!.Issuer,
                ValidAudience = jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key!))
            };
            options.Events = new JwtBearerEvents
            {
                OnChallenge = async context =>
                {
                    // Ekstra håndtering ved uatoriserte forsøk ved å gi response body, og logging
                    context.HandleResponse();
                    
                    ILogger<Program> logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                    logger.LogWarning("Uatorisert forsøk. URL: {Path}, Scheme: {Scheme}, IP: {IP}",
                        context.HttpContext.Request.Path,
                        context.HttpContext.Request.Scheme,
                        context.HttpContext.Connection.RemoteIpAddress);
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";

                    await context.Response.WriteAsJsonAsync(new
                    {
                        Message = "Du er ikke autorisert til å bruke denne ressursen",
                        ErrorCode = StatusCodes.Status401Unauthorized
                    });
                },
            };
        });
        
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserContextService, UserContextService>();
        services.AddHttpContextAccessor();

    }
    
    // Fluent API for datavalidering
    public static void ConfigureFluentValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<Program>();
        services.AddFluentValidationAutoValidation(options =>
            options.DisableDataAnnotationsValidation = true);
    }
    
    // Oppsett av database med feilhåndtering og ekstra forsøk på tilkobling ved feil
    public static void AddDatabaseService(this IServiceCollection services, IConfiguration configuration)
    {
        try
        {
            services.AddDbContext<GaaDbContext>(options =>
                options.UseMySql(configuration.GetConnectionString("DefaultConnection"),
                    ServerVersion.AutoDetect(configuration.GetConnectionString("DefaultConnection")),
                    mySqlOptions => mySqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorNumbersToAdd: null)));
        }
        catch (MySqlException)
        {
            throw new DatabaseUnavailableException("Forsøk på å nå database feilet. Vennligst prøv igjen senere.");
        }
        catch (Exception)
        {
            throw new DatabaseUnavailableException("Forsøk på å koble til database feilet.");
        }
    }
    
    // Database- helsesjekk
    public static void AddDatabaseHealthCheck(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddHealthChecks()
            .AddMySql(configuration.GetConnectionString("DefaultConnection")!);
    }
    
    // Feilhåndtering
    public static void ConfigureExceptionHandler(this IServiceCollection services)
    {
        services.AddExceptionHandler<ExceptionHandling>();
    }
    
    // Registrering av mappere à la Yngve
    public static void RegisterMappers(this IServiceCollection services)
    {
        Assembly assembly = typeof(MemberMapper).Assembly;

        List<Type> mapperTypes = assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } && t.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapper<,>)))
            .ToList();

        foreach (Type mapperType in mapperTypes)
        {
            Type interfaceType = mapperType.GetInterfaces().First(i => i.GetGenericTypeDefinition() == typeof(IMapper<,>));
            services.AddScoped(interfaceType, mapperType);
        }
    }

    // Registrering av services à la Yngve
    public static void RegisterServices(this IServiceCollection services)
    {
        Assembly assembly = typeof(MemberService).Assembly;

        List<Type> serviceTypes = assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } && t.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IService<>)))
            .ToList();

        foreach (Type serviceType in serviceTypes)
        {
            Type interfaceType = serviceType.GetInterfaces().First();
            services.AddScoped(interfaceType, serviceType);
        }
    }

    // Registrering av repositories à la Yngve
    public static void RegisterRepositories(this IServiceCollection services)
    {
        Assembly assembly = typeof(MemberRepository).Assembly;
    
        List<Type> reposTypes = assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } && t.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRepositry<>)))
            .ToList();
    
        foreach (Type repoType in reposTypes)
        {
            Type interfaceType = repoType.GetInterfaces().First();
            services.AddScoped(interfaceType, repoType);
        }
    }
}