using GokstadFriidrettsforeningAPI.Extensions;
using GokstadFriidrettsforeningAPI.Middleware;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerJwtAuthentication();
builder.Services.ConfigureAuthentication(builder.Configuration);
builder.Services.ConfigureFluentValidation();
builder.Services.AddDatabaseService(builder.Configuration);
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

app.UseMiddleware<DatabaseConnectionMiddleware>();
app.UseExceptionHandler(_ => { });
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();