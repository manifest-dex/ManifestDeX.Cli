namespace ManifestDeX.Cli.Domain.Ports;

public interface IApiKeyStore
{
    Task SetApiKeyAsync(string apiKey, CancellationToken cancellationToken = default);
    Task<string?> GetApiKeyAsync(CancellationToken cancellationToken = default);
}
