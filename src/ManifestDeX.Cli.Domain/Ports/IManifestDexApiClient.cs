using ManifestDeX.Cli.Domain.Entities;

namespace ManifestDeX.Cli.Domain.Ports;

public interface IManifestDexApiClient
{
    Task<IReadOnlyList<SearchGameResult>> SearchAsync(string query, CancellationToken cancellationToken = default);
    Task<GameInfo> GetGameInfoAsync(uint appId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DepotKey>> GetKeysAsync(uint appId, CancellationToken cancellationToken = default);
    Task<UsageSnapshot> GetUsageAsync(CancellationToken cancellationToken = default);
    Task<HealthSnapshot> GetHealthAsync(CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<Entities.AvailableManifest>> GetAvailableManifestsAsync(uint appId, uint[]? depotIds = null, CancellationToken cancellationToken = default);
    Task<Entities.DownloadQueueResponse> PrepareDownloadAsync(uint? appId, List<uint>? depotIds, List<Entities.AvailableManifest>? manifests, CancellationToken cancellationToken = default);
    Task<Entities.DownloadQueueResponse> GetDownloadStatusAsync(string taskId, CancellationToken cancellationToken = default);
    Task<Stream> DownloadStreamAsync(string downloadUrl, CancellationToken cancellationToken = default);

    Task<CliPaginatedList<OnlineFixListItem>> ListOnlineFixesAsync(int page, int pageSize, string? search = null, CancellationToken cancellationToken = default);
    Task<CliPaginatedList<BypassListItem>> ListBypassesAsync(int page, int pageSize, string? search = null, CancellationToken cancellationToken = default);
    Task<DownloadLink> GetOnlineFixDownloadLinkAsync(uint appId, CancellationToken cancellationToken = default);
    Task<DownloadLink> GetBypassDownloadLinkAsync(uint appId, CancellationToken cancellationToken = default);
}
