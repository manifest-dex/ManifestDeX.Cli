using ManifestDeX.Cli.Domain.Ports;

namespace ManifestDeX.Cli.Application.UseCases;

public sealed class GetHealthUseCase
{
    private readonly IManifestDexApiClient _apiClient;

    public GetHealthUseCase(IManifestDexApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<Domain.Entities.HealthSnapshot> ExecuteAsync(CancellationToken cancellationToken = default)
        => _apiClient.GetHealthAsync(cancellationToken);
}
