using ManifestDeX.Cli.Domain.Ports;
using ManifestDeX.Cli.Domain.Entities;

namespace ManifestDeX.Cli.Application.UseCases;

public sealed class GetOnlineFixDownloadLinkUseCase
{
    private readonly IManifestDexApiClient _apiClient;

    public GetOnlineFixDownloadLinkUseCase(IManifestDexApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<DownloadLink> ExecuteAsync(uint appId, CancellationToken cancellationToken = default)
    {
        if (appId == 0)
        {
            throw new Errors.CliException("appId must be greater than 0.", Errors.CliExitCode.ValidationError);
        }

        return _apiClient.GetOnlineFixDownloadLinkAsync(appId, cancellationToken);
    }
}
