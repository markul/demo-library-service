using LibraryService.Application.Abstractions.Repositories;
using LibraryService.Application.Abstractions.Services;
using LibraryService.Infrastructure.ConnectedServices.EbookOData;
using LibraryService.Infrastructure.Database;
using LibraryService.Infrastructure.Repositories;
using LibraryService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentService.Client;

namespace LibraryService.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("AppDb")
            ?? "Host=app-db;Port=5432;Database=appdb;Username=app;Password=app";

        services.AddDbContext<LibraryDbContext>(options => options.UseNpgsql(connectionString));

        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IJournalRepository, JournalRepository>();
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
        services.AddScoped<ISubscriptionTypeRepository, SubscriptionTypeRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddHttpClient<IPaymentServiceClient, PaymentServiceClient>((_, client) =>
        {
            var baseUrl = configuration["PaymentService:BaseUrl"] ?? "http://localhost:8082/";
            client.BaseAddress = new Uri(baseUrl);
        });
        services.AddScoped(_ =>
        {
            var baseUrl = configuration["EbookService:BaseUrl"] ?? "http://localhost:8083/";
            var serviceRoot = BuildEbookServiceRoot(baseUrl);

            return new EbookContainer(serviceRoot);
        });
        services.AddScoped<IPaymentService, Services.PaymentService>();
        services.AddScoped<IEbookCatalogService, EbookCatalogService>();

        return services;
    }

    private static Uri BuildEbookServiceRoot(string baseUrl)
    {
        var normalizedBaseUrl = baseUrl.EndsWith("/", StringComparison.Ordinal)
            ? baseUrl
            : $"{baseUrl}/";

        return new Uri(new Uri(normalizedBaseUrl, UriKind.Absolute), "odata/");
    }
}

