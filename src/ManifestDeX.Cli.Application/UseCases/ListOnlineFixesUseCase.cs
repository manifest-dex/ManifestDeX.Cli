using ManifestDeX.Cli.Domain.Ports;
using ManifestDeX.Cli.Domain.Entities;

namespace ManifestDeX.Cli.Application.UseCases;

public sealed class ListOnlineFixesUseCase
{
    private readonly IManifestDexApiClient _apiClient;

    public ListOnlineFixesUseCase(IManifestDexApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<CliPaginatedList<OnlineFixListItem>> ExecuteAsync(int page, int pageSize, string? search = null, CancellationToken cancellationToken = default)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0 || pageSize > 100) pageSize = 100;

        return _apiClient.ListOnlineFixesAsync(page, pageSize, search, cancellationToken);
    }
}
