using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ManifestDeX.Cli.Application.Errors;
using ManifestDeX.Cli.Domain.Entities;
using ManifestDeX.Cli.Domain.Ports;

namespace ManifestDeX.Cli.Infrastructure.Adapters;

public sealed class ManifestDexApiClient : IManifestDexApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IApiKeyStore _apiKeyStore;

    public ManifestDexApiClient(HttpClient httpClient, IApiKeyStore apiKeyStore)
    {
        _httpClient = httpClient;
        _apiKeyStore = apiKeyStore;
    }

    public async Task<IReadOnlyList<SearchGameResult>> SearchAsync(string query, CancellationToken cancellationToken = default)
    {
        var response = await SendAuthenticatedAsync($"api/cli/search?query={Uri.EscapeDataString(query)}", cancellationToken);
        var payload = await response.Content.ReadFromJsonAsync<SearchResponse>(JsonOptions, cancellationToken)
            ?? throw new CliException("Invalid server response.", CliExitCode.UnknownError);

        return payload.Results.Select(x => new SearchGameResult(x.AppId, x.Name, x.AvailableDecryptionKeys)).ToList();
    }

    public async Task<GameInfo> GetGameInfoAsync(uint appId, CancellationToken cancellationToken = default)
    {
        var response = await SendAuthenticatedAsync($"api/cli/info/{appId}", cancellationToken);
        var payload = await response.Content.ReadFromJsonAsync<GameInfoResponse>(JsonOptions, cancellationToken)
            ?? throw new CliException("Invalid server response.", CliExitCode.UnknownError);

        return new GameInfo(payload.AppId, payload.Name, payload.HeaderImageUrl, payload.TotalDecryptionKeys, payload.DepotIds);
    }

    public async Task<IReadOnlyList<DepotKey>> GetKeysAsync(uint appId, CancellationToken cancellationToken = default)
    {
        var response = await SendAuthenticatedAsync($"api/cli/get/{appId}", cancellationToken);
        var payload = await response.Content.ReadFromJsonAsync<GetResponse>(JsonOptions, cancellationToken)
            ?? throw new CliException("Invalid server response.", CliExitCode.UnknownError);

        return payload.Keys.Select(k => new DepotKey(appId, k.DepotId, k.Key)).ToList();
    }

    public async Task<UsageSnapshot> GetUsageAsync(CancellationToken cancellationToken = default)
    {
        var response = await SendAuthenticatedAsync("api/cli/usage", cancellationToken);
        var payload = await response.Content.ReadFromJsonAsync<UsageResponse>(JsonOptions, cancellationToken)
            ?? throw new CliException("Invalid server response.", CliExitCode.UnknownError);

        return new UsageSnapshot(payload.DailyLimit, payload.UsedToday, payload.RemainingToday, payload.ResetAtUtc);
    }

    public async Task<HealthSnapshot> GetHealthAsync(CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response;
        try
        {
            response = await _httpClient.GetAsync("api/cli/health", cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            throw new CliException("Network error while contacting ManifestDeX API.", CliExitCode.NetworkError, ex);
        }

        // Some deployments protect /api/cli/health with API key auth.
        if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
        {
            response = await SendAuthenticatedAsync("api/cli/health", cancellationToken);
        }
        else if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var errorMessage = TryExtractErrorMessage(content) ?? $"Request failed with HTTP {(int)response.StatusCode}.";
            throw new CliException(errorMessage, CliExitCode.UnknownError);
        }

        var payload = await response.Content.ReadFromJsonAsync<HealthResponse>(JsonOptions, cancellationToken)
            ?? throw new CliException("Invalid server response.", CliExitCode.UnknownError);

        return new HealthSnapshot(payload.Status, payload.TimestampUtc);
    }

    private async Task<HttpResponseMessage> SendAuthenticatedAsync(string path, CancellationToken cancellationToken)
    {
        var key = await _apiKeyStore.GetApiKeyAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new CliException("API key not configured. Run: manifestdex auth set-key <key>", CliExitCode.ValidationError);
        }

        using var request = new HttpRequestMessage(HttpMethod.Get, path);
        request.Headers.Add("X-Manifestdex-Key", key);

        HttpResponseMessage response;
        try
        {
            response = await _httpClient.SendAsync(request, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            throw new CliException("Network error while contacting ManifestDeX API.", CliExitCode.NetworkError, ex);
        }

        if (response.IsSuccessStatusCode)
        {
            return response;
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var errorMessage = TryExtractErrorMessage(content) ?? $"Request failed with HTTP {(int)response.StatusCode}.";

        throw response.StatusCode switch
        {
            HttpStatusCode.Unauthorized => new CliException(errorMessage, CliExitCode.Unauthorized),
            HttpStatusCode.Forbidden => new CliException(errorMessage, CliExitCode.Forbidden),
            (HttpStatusCode)429 => new CliException(errorMessage, CliExitCode.RateLimited),
            _ => new CliException(errorMessage, CliExitCode.UnknownError)
        };
    }

    private static string? TryExtractErrorMessage(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return null;
        }

        try
        {
            using var doc = JsonDocument.Parse(content);
            if (doc.RootElement.TryGetProperty("error", out var error))
            {
                return error.GetString();
            }
        }
        catch
        {
            // ignore
        }

        return null;
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private sealed class SearchResponse
    {
        public bool Success { get; set; }
        public List<SearchItem> Results { get; set; } = [];
    }

    private sealed class SearchItem
    {
        public uint AppId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int AvailableDecryptionKeys { get; set; }
    }

    private sealed class GameInfoResponse
    {
        public bool Success { get; set; }
        public uint AppId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string HeaderImageUrl { get; set; } = string.Empty;
        public int TotalDecryptionKeys { get; set; }
        public List<uint> DepotIds { get; set; } = [];
    }

    private sealed class GetResponse
    {
        public bool Success { get; set; }
        public List<DecryptionKey> Keys { get; set; } = [];
    }

    private sealed class DecryptionKey
    {
        public uint DepotId { get; set; }
        public string Key { get; set; } = string.Empty;
    }

    private sealed class UsageResponse
    {
        public bool Success { get; set; }
        public int DailyLimit { get; set; }
        public int UsedToday { get; set; }
        public int RemainingToday { get; set; }
        public DateTime ResetAtUtc { get; set; }
    }

    private sealed class HealthResponse
    {
        public bool Success { get; set; }
        public string Status { get; set; } = "unknown";
        public DateTime TimestampUtc { get; set; }
    }
}
