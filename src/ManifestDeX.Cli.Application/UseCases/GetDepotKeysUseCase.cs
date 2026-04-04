using ManifestDeX.Cli.Domain.Ports;

namespace ManifestDeX.Cli.Application.UseCases;

public sealed class GetDepotKeysUseCase
{
    private readonly IManifestDexApiClient _apiClient;

    public GetDepotKeysUseCase(IManifestDexApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<IReadOnlyList<Domain.Entities.DepotKey>> ExecuteAsync(uint appId, CancellationToken cancellationToken = default)
    {
        if (appId == 0)
        {
            throw new Errors.CliException("appId must be greater than 0.", Errors.CliExitCode.ValidationError);
        }

        return _apiClient.GetKeysAsync(appId, cancellationToken);
    }
}
