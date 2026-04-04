using ManifestDeX.Cli.Infrastructure.Adapters;

namespace ManifestDeX.Cli.IntegrationTests;

public class JsonFileApiKeyStoreTests
{
    [Fact]
    public async Task SetAndGetApiKey_ShouldPersistValue()
    {
        var store = new JsonFileApiKeyStore();
        const string key = "mx_cli_test_key";

        await store.SetApiKeyAsync(key);
        var loaded = await store.GetApiKeyAsync();

        Assert.Equal(key, loaded);
    }
}
