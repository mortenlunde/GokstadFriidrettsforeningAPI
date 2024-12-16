using GokstadFriidrettsforeningAPI.Extensions;
using GokstadFriidrettsforeningAPI.Middleware;
using GokstadFriidrettsforeningAPI.Services;
using Newtonsoft.Json;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

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
builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = JsonConvert.SerializeObject(new
        {
            status = report.Status.ToString(),
            errors = report.Entries.Select(e => new
            {
                key = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description
            })
        });
        await context.Response.WriteAsync(result);
    }
});
app.UseExceptionHandler(_ => { });
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();