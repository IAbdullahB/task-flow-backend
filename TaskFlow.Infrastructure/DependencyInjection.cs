using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using TaskFlow.Application.ServicesInterfaces;
using TaskFlow.Domain.ReposInterfaces;
using TaskFlow.Infrastructure.Repositories;
using TaskFlow.Infrastructure.Services;
using TaskFlow.Infrastructure.Settings;
using TaskFlow.Infrastructure.UseCase;

namespace TaskFlow.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("TaskFlow.Host")
            ));

        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>()
            ?? throw new InvalidOperationException("JwtSettings section is missing from configuration.");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = jwtSettings.ValidateIssuer,
                ValidIssuer = jwtSettings.ValidIssuer,

                ValidateAudience = jwtSettings.ValidateAudience,
                ValidAudience = jwtSettings.ValidAudience,

                ValidateLifetime = jwtSettings.ValidateLifetime,
                RequireExpirationTime = jwtSettings.RequireExpirationTime,

                ValidateIssuerSigningKey = jwtSettings.ValidateIssuerSigningKey,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),

                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddMemoryCache();

        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<ICacheService, CacheService>();
        services.AddScoped<AuthenticationUseCase>();
        services.AddScoped<TaskItemsUseCase>();
        services.AddScoped<UsersUseCase>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITaskItemRepository, TaskItemRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}