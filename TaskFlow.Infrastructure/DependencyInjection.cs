using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskFlow.Infrastructure.Services;
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
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)
            ));

        services.AddScoped<JwtService>();
        services.AddScoped<CacheService>();

        services.AddScoped<AuthenticationUseCase>();
        services.AddScoped<TaskItemsUseCase>();
        services.AddScoped<UsersUseCase>();

        return services;
    }
}