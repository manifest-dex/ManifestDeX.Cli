using ManifestDeX.Cli.Domain.Ports;

namespace ManifestDeX.Cli.Application.UseCases;

public sealed class GetGameInfoUseCase
{
    private readonly IManifestDexApiClient _apiClient;

    public GetGameInfoUseCase(IManifestDexApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<Domain.Entities.GameInfo> ExecuteAsync(uint appId, CancellationToken cancellationToken = default)
    {
        if (appId == 0)
        {
            throw new Errors.CliException("appId must be greater than 0.", Errors.CliExitCode.ValidationError);
        }

        return _apiClient.GetGameInfoAsync(appId, cancellationToken);
    }
}
