using ManifestDeX.Cli.Domain.Ports;

namespace ManifestDeX.Cli.Application.UseCases;

public sealed class SearchGameUseCase
{
    private readonly IManifestDexApiClient _apiClient;

    public SearchGameUseCase(IManifestDexApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<IReadOnlyList<Domain.Entities.SearchGameResult>> ExecuteAsync(string query, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            throw new Errors.CliException("Search query is required.", Errors.CliExitCode.ValidationError);
        }

        return _apiClient.SearchAsync(query.Trim(), cancellationToken);
    }
}
