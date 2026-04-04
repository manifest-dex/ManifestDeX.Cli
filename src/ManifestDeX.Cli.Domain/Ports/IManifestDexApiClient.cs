using ManifestDeX.Cli.Domain.Entities;

namespace ManifestDeX.Cli.Domain.Ports;

public interface IManifestDexApiClient
{
    Task<IReadOnlyList<SearchGameResult>> SearchAsync(string query, CancellationToken cancellationToken = default);
    Task<GameInfo> GetGameInfoAsync(uint appId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DepotKey>> GetKeysAsync(uint appId, CancellationToken cancellationToken = default);
    Task<UsageSnapshot> GetUsageAsync(CancellationToken cancellationToken = default);
    Task<HealthSnapshot> GetHealthAsync(CancellationToken cancellationToken = default);
}
