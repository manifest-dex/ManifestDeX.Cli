using ManifestDeX.Cli.Presentation.Commands;
using Spectre.Console.Cli;

namespace ManifestDeX.Cli.Presentation;

public static class SpectreCliConfigurator
{
    public static void Configure(IConfigurator config)
    {
        config.SetApplicationName("manifestdex");

        config.AddCommand<SearchCommand>("search")
            .WithDescription("Search games by name and return appId + key count.")
            .WithAlias("find")
            .WithAlias("s");

        config.AddCommand<InfoCommand>("info")
            .WithDescription("Get game details for an appId.")
            .WithAlias("i");

        config.AddCommand<GetCommand>("get")
            .WithDescription("Get available depot keys for an appId.")
            .WithAlias("keys")
            .WithAlias("g");

        config.AddCommand<ManifestFileCommand>("manifestfile")
            .WithDescription("Download .manifest files (either by game AppID or specific versions).")
            .WithAlias("mf");

        config.AddCommand<UsageCommand>("usage")
            .WithDescription("Show daily usage and reset time.")
            .WithAlias("quota")
            .WithAlias("u");

        config.AddCommand<HealthCommand>("health")
            .WithDescription("Check CLI API health.")
            .WithAlias("ping");

        config.AddCommand<HelpCommand>("help")
            .WithDescription("Show detailed usage documentation and examples.")
            .WithAlias("h");

        config.AddBranch("auth", auth =>
        {
            auth.SetDescription("Authentication and API key operations.");
            auth.AddCommand<AuthSetKeyCommand>("set-key")
                .WithDescription("Store API key locally.")
                .WithAlias("set");
            auth.AddCommand<AuthStatusCommand>("status")
                .WithDescription("Validate stored API key and show usage.")
                .WithAlias("check");
        });

        config.AddExample(["search", "elden ring"]);
        config.AddExample(["find", "counter-strike"]);
        config.AddExample(["info", "730"]);
        config.AddExample(["get", "730"]);
        config.AddExample(["manifestfile", "730"]);
        config.AddExample(["mf", "730", "--depots", "731,732"]);
        config.AddExample(["mf", "--manifests", "731:89273928172938,732:89239823982389", "--zip"]);
        config.AddExample(["auth", "set-key", "mx_cli_xxx"]);
        config.AddExample(["auth", "status"]);
        config.AddExample(["usage"]);
        config.AddExample(["usage", "--json"]);
        config.AddExample(["health"]);
        config.AddExample(["help"]);
    }
}
