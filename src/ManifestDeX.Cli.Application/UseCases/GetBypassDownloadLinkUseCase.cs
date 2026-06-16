using ManifestDeX.Cli.Domain.Ports;
using ManifestDeX.Cli.Domain.Entities;

namespace ManifestDeX.Cli.Application.UseCases;

public sealed class GetBypassDownloadLinkUseCase
{
    private readonly IManifestDexApiClient _apiClient;

    public GetBypassDownloadLinkUseCase(IManifestDexApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<DownloadLink> ExecuteAsync(uint appId, CancellationToken cancellationToken = default)
    {
        if (appId == 0)
        {
            throw new Errors.CliException("appId must be greater than 0.", Errors.CliExitCode.ValidationError);
        }

        return _apiClient.GetBypassDownloadLinkAsync(appId, cancellationToken);
    }
}
