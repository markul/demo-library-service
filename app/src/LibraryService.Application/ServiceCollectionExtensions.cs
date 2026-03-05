using LibraryService.Application.Status;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryService.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly));
        services.AddScoped<StatusService>();
        return services;
    }
}
