using CampusRouteLab.Services;

namespace CampusRouteLab.Services;

public static class ServiceCollectionExtensions 
{ 
    public static IServiceCollection AddCampusServices(this IServiceCollection services)
    {
        // Singleton — один экземпляр на всё приложение
        services.AddSingleton<IAppInfoService, AppInfoService>();
        services.AddSingleton<IStudentCatalogService, StudentCatalogService>();

        // Scoped — один экземпляр на каждый HTTP-запрос
        services.AddScoped<IRequestContextService, RequestContextService>();
        services.AddScoped<DiagnosticsReportService>();

        // Transient — новый экземпляр при каждом обращении
        services.AddTransient<ITransientMarkerService, TransientMarkerService>();

        return services;
    } 
}