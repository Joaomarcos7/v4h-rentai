using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using V4H.Application.Common.Interfaces;
using V4H.Domain.Interfaces.Repositories;
using V4H.Infrastructure.Hubs;
using V4H.Infrastructure.Persistence;
using V4H.Infrastructure.Persistence.Repositories;
using V4H.Infrastructure.Services;

namespace V4H.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(opts =>
            opts.UseNpgsql(config.GetConnectionString("Default")));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITeleconsultoriaRepository, TeleconsultoriaRepository>();
        services.AddScoped<IOpinionRepository, OpinionRepository>();

        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IDocumentValidationService, MockDocumentValidationService>();
        services.AddScoped<IFileStorageService, LocalFileStorageService>();
        services.AddScoped<IPdfExportService, QuestPdfExportService>();
        services.AddScoped<INotificationService, SignalRNotificationService>();

        services.AddSignalR();

        return services;
    }
}
