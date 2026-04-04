using ManifestDeX.Cli.Domain.Ports;

namespace ManifestDeX.Cli.Application.UseCases;

public sealed class SetApiKeyUseCase
{
    private readonly IApiKeyStore _apiKeyStore;

    public SetApiKeyUseCase(IApiKeyStore apiKeyStore)
    {
        _apiKeyStore = apiKeyStore;
    }

    public async Task ExecuteAsync(string apiKey, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new Errors.CliException("API key is required.", Errors.CliExitCode.ValidationError);
        }

        await _apiKeyStore.SetApiKeyAsync(apiKey.Trim(), cancellationToken);
    }
}
