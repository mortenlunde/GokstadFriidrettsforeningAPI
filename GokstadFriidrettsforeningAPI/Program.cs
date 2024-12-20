using GokstadFriidrettsforeningAPI.Extensions;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Tjenester som registreres i applikasjonen
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerJwtAuthentication();
builder.Services.ConfigureAuthentication(builder.Configuration);
builder.Services.ConfigureFluentValidation();
builder.Services.AddDatabaseService(builder.Configuration);
builder.Services.AddDatabaseHealthCheck(builder.Configuration);
builder.Services.RegisterMappers();
builder.Services.RegisterServices();
builder.Services.RegisterRepositories();
builder.Services.ConfigureExceptionHandler();

// Serilog-konfigurasjon
builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

WebApplication app = builder.Build();

app
    .UseSwagger()
    .UseSwaggerUI();

// Database Helsesjekk
app.UseHealthChecks("/health");

// Global feilhåndtering
app.UseExceptionHandler(_ => { });
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// For at integrasjonstest skal fungere
public partial class Program { }