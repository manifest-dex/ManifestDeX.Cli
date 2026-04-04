using System.Text.Json;
using System.Text.Json.Serialization;
using ManifestDeX.Cli.Domain.Ports;

namespace ManifestDeX.Cli.Infrastructure.Adapters;

public sealed class JsonFileApiKeyStore : IApiKeyStore
{
    private readonly string _configFilePath;

    public JsonFileApiKeyStore()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var dir = Path.Combine(appData, "ManifestDeX");
        Directory.CreateDirectory(dir);
        _configFilePath = Path.Combine(dir, "cli-config.json");
    }

    public async Task SetApiKeyAsync(string apiKey, CancellationToken cancellationToken = default)
    {
        var model = await ReadAsync(cancellationToken) ?? new CliConfig();
        model.ApiKey = apiKey;

        await using var stream = File.Create(_configFilePath);
        await JsonSerializer.SerializeAsync(stream, model, JsonOptions, cancellationToken);
    }

    public async Task<string?> GetApiKeyAsync(CancellationToken cancellationToken = default)
    {
        var model = await ReadAsync(cancellationToken);
        return model?.ApiKey;
    }

    private async Task<CliConfig?> ReadAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(_configFilePath))
        {
            return null;
        }

        await using var stream = File.OpenRead(_configFilePath);
        return await JsonSerializer.DeserializeAsync<CliConfig>(stream, JsonOptions, cancellationToken);
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private sealed class CliConfig
    {
        public string? ApiKey { get; set; }
    }
}
