using ManifestDeX.Cli.Domain.Ports;

namespace ManifestDeX.Cli.Application.UseCases;

public sealed class GetUsageUseCase
{
    private readonly IManifestDexApiClient _apiClient;

    public GetUsageUseCase(IManifestDexApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<Domain.Entities.UsageSnapshot> ExecuteAsync(CancellationToken cancellationToken = default)
        => _apiClient.GetUsageAsync(cancellationToken);
}
