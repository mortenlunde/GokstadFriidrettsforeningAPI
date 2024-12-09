using GokstadFriidrettsforeningAPI.Data;
using Microsoft.EntityFrameworkCore;
using FluentValidation.AspNetCore;
using GokstadFriidrettsforeningAPI.Features.Repositories;
using GokstadFriidrettsforeningAPI.Features.Services;
using GokstadFriidrettsforeningAPI.Features.Services.Interfaces;
using GokstadFriidrettsforeningAPI.Mappers;
using GokstadFriidrettsforeningAPI.ModelResponses;
using GokstadFriidrettsforeningAPI.Models;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<GaaDbContext>(options =>
{
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), 
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection")),
    mysqloptions => mysqloptions.EnableRetryOnFailure(2));
});
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddScoped<IMapper<Member, MemberResponse>, MemberMapper>();
builder.Services.AddScoped<IMapper<Member, MemberRegistration>, MemberRegMapper>();

WebApplication app = builder.Build();

using (IServiceScope scope = app.Services.CreateScope())
{
    GaaDbContext dbContext = scope.ServiceProvider.GetRequiredService<GaaDbContext>();
    dbContext.Database.Migrate(); // Auto-apply migrations and create the database
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();