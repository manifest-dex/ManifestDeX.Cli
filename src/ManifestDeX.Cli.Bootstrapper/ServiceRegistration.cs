using ManifestDeX.Cli.Application.UseCases;
using ManifestDeX.Cli.Domain.Ports;
using ManifestDeX.Cli.Infrastructure.Adapters;
using Microsoft.Extensions.DependencyInjection;

namespace ManifestDeX.Cli.Bootstrapper;

public static class ServiceRegistration
{
    public static IServiceCollection CreateServiceCollection()
    {
        var services = new ServiceCollection();

        var apiBaseUrl = Environment.GetEnvironmentVariable("MANIFESTDEX_API_URL")?.Trim();
        if (string.IsNullOrWhiteSpace(apiBaseUrl))
        {
            apiBaseUrl = "https://api.manifestdex.com/";
        }

        if (!apiBaseUrl.EndsWith('/'))
        {
            apiBaseUrl += "/";
        }

        services.AddSingleton<IApiKeyStore, JsonFileApiKeyStore>();
        services.AddHttpClient<IManifestDexApiClient, ManifestDexApiClient>(client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl, UriKind.Absolute);
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddTransient<SearchGameUseCase>();
        services.AddTransient<GetGameInfoUseCase>();
        services.AddTransient<GetDepotKeysUseCase>();
        services.AddTransient<GetUsageUseCase>();
        services.AddTransient<GetHealthUseCase>();
        services.AddTransient<SetApiKeyUseCase>();
        services.AddTransient<GetAuthStatusUseCase>();
        services.AddTransient<DownloadManifestsUseCase>();
        services.AddTransient<ListOnlineFixesUseCase>();
        services.AddTransient<ListBypassesUseCase>();
        services.AddTransient<GetOnlineFixDownloadLinkUseCase>();
        services.AddTransient<GetBypassDownloadLinkUseCase>();

        return services;
    }

    public static ServiceProvider BuildServiceProvider()
        => CreateServiceCollection().BuildServiceProvider();
}
