using System.Net;
using System.Text;
using System.Text.Json;
using ManifestDeX.Cli.Domain.Entities;
using ManifestDeX.Cli.Domain.Ports;
using ManifestDeX.Cli.Infrastructure.Adapters;
using ManifestDeX.Cli.Presentation.Output;
using ManifestDeX.Cli.Presentation.Parsing;
using Spectre.Console;

namespace ManifestDeX.Cli.UnitTests;

public class BypassArchiveTests
{
    [Theory]
    [InlineData("true", "\"manifestdex.com\"", true, "manifestdex.com")]
    [InlineData("false", "null", false, null)]
    [InlineData("null", "null", null, null)]
    [InlineData(null, null, null, null)]
    public async Task ApiClient_PreservesMetadataAcrossBypassEndpoints(
        string? statusJson, string? passwordJson, bool? expectedStatus, string? expectedPassword)
    {
        var fields = statusJson == null ? "" : $",\"isPasswordProtected\":{statusJson},\"archivePassword\":{passwordJson}";
        var handler = new StubHandler(path => path.Contains("info/")
            ? "{\"success\":true,\"appId\":730,\"bypassDetails\":{\"fileSize\":10,\"additionalInfo\":\"test\",\"lastUpdated\":\"2026-10-20T00:00:00Z\"" + fields + "}}"
            : path.Contains("/list")
                ? "{\"success\":true,\"results\":[{\"appId\":730,\"name\":\"test\",\"additionalInfo\":\"test\"" + fields + "}],\"page\":1,\"pageSize\":100,\"totalCount\":1,\"totalPages\":1}"
                : "{\"success\":true,\"downloadUrl\":\"https://example.com/archive.zip\"" + fields + "}");
        using var http = new HttpClient(handler) { BaseAddress = new Uri("https://example.com/") };
        var client = new ManifestDexApiClient(http, new StubKeyStore());
        var info = (await client.GetGameInfoAsync(730)).BypassDetails!;
        var item = Assert.Single((await client.ListBypassesAsync(1, 100)).Results);
        var link = await client.GetBypassDownloadLinkAsync(730);
        Assert.Equal(expectedStatus, info.IsPasswordProtected);
        Assert.Equal(expectedPassword, info.ArchivePassword);
        Assert.Equal(expectedStatus, item.IsPasswordProtected);
        Assert.Equal(expectedPassword, item.ArchivePassword);
        Assert.Equal(expectedStatus, link.IsPasswordProtected);
        Assert.Equal(expectedPassword, link.ArchivePassword);
        using var json = JsonDocument.Parse(JsonSerializer.Serialize(link));
        Assert.Equal(expectedPassword, json.RootElement.GetProperty("ArchivePassword").GetString());
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    [InlineData(null)]
    public void ConsoleOutput_ShowsPasswordOnlyForEncryptedArchives(bool? encrypted)
    {
        using var writer = new StringWriter();
        var original = AnsiConsole.Console;
        try
        {
            AnsiConsole.Console = AnsiConsole.Create(new AnsiConsoleSettings
            {
                Ansi = AnsiSupport.No,
                ColorSystem = ColorSystemSupport.NoColors,
                Out = new AnsiConsoleOutput(writer)
            });
            ConsoleOutput.PrintObject(new DownloadLink("https://example.com/archive.zip", encrypted, "manifestdex.com"), OutputMode.Table);
            ConsoleOutput.PrintObject(new GameInfo(730, "Test", "", 0, [], BypassDetails:
                new BypassDetails(10, "test", DateTime.UtcNow, encrypted, "manifestdex.com")), OutputMode.Table);
            ConsoleOutput.PrintObject(new CliPaginatedList<BypassListItem>(
                [new BypassListItem(730, "Test", "test", encrypted, "manifestdex.com")], 1, 100, 1, 1), OutputMode.Table);
        }
        finally { AnsiConsole.Console = original; }
        Assert.Equal(encrypted == true, writer.ToString().Contains("manifestdex.com"));
        if (encrypted == null) Assert.Contains("Unknown", writer.ToString());
        if (encrypted == false) Assert.Contains("Unencrypted", writer.ToString());
    }

    private sealed class StubHandler(Func<string, string> respond) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(respond(request.RequestUri!.AbsolutePath), Encoding.UTF8, "application/json")
            });
    }

    private sealed class StubKeyStore : IApiKeyStore
    {
        public Task<string?> GetApiKeyAsync(CancellationToken cancellationToken = default) => Task.FromResult<string?>("test-key");
        public Task SetApiKeyAsync(string apiKey, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
}
