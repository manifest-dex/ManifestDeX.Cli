using ManifestDeX.Cli.Domain.Ports;

namespace ManifestDeX.Cli.Application.UseCases;

public sealed class GetAuthStatusUseCase
{
    private readonly IApiKeyStore _apiKeyStore;
    private readonly IManifestDexApiClient _apiClient;

    public GetAuthStatusUseCase(IApiKeyStore apiKeyStore, IManifestDexApiClient apiClient)
    {
        _apiKeyStore = apiKeyStore;
        _apiClient = apiClient;
    }

    public async Task<(bool hasKey, Domain.Entities.UsageSnapshot? usage)> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var key = await _apiKeyStore.GetApiKeyAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(key))
        {
            return (false, null);
        }

        var usage = await _apiClient.GetUsageAsync(cancellationToken);
        return (true, usage);
    }
}
