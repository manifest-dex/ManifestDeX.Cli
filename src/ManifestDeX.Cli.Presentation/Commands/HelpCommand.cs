using Spectre.Console;
using Spectre.Console.Cli;

namespace ManifestDeX.Cli.Presentation.Commands;

public sealed class HelpCommand : Command<HelpCommand.Settings>
{
    public sealed class Settings : CommandSettings;

    protected override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        AnsiConsole.Write(
            new Panel(new Markup(
                "[bold deepskyblue1]ManifestDeX CLI[/]\n" +
                "[grey]Query ManifestDeX data from terminal with API key authentication.[/]"))
            .Header("[blue]Overview[/]")
            .Border(BoxBorder.Rounded));

        var setup = new Table().RoundedBorder().AddColumns("Step", "Command");
        setup.AddRow("1. Save your API key", "[green]manifestdex auth set-key <your_key>[/]");
        setup.AddRow("2. Check authentication", "[green]manifestdex auth status[/]");
        setup.AddRow("3. Start querying", "[green]manifestdex search \"game name\"[/]");
        AnsiConsole.Write(setup);
        AnsiConsole.WriteLine();

        var commands = new Table().RoundedBorder().AddColumns("Command", "Aliases", "Description");
        commands.AddRow("search <query>", "find, s", "Search games and list appId + available key count.");
        commands.AddRow("info <appId>", "i", "Show detailed game information.");
        commands.AddRow("get <appId>", "keys, g", "List depot decryption keys (depotId:key).");
        commands.AddRow("manifestfile [appId]", "mf", "Download .manifest files (either by AppID or custom list of specific versions).");
        commands.AddRow("online-fix list", "online-fix l", "List games with online-fix available (paginated, free).");
        commands.AddRow("online-fix download <appId>", "online-fix d", "Generate temporary download link for online-fix (costs 1 credit).");
        commands.AddRow("bypass list", "bypass l", "List games with bypass available (paginated, free).");
        commands.AddRow("bypass download <appId>", "bypass d", "Generate temporary download link for bypass (costs 1 credit).");
        commands.AddRow("usage", "quota, u", "Show daily limit, usage, remaining quota, reset time.");
        commands.AddRow("health", "ping", "Check CLI API availability.");
        commands.AddRow("auth set-key <key>", "auth set", "Store API key locally.");
        commands.AddRow("auth status", "auth check", "Validate local key and show current usage.");
        commands.AddRow("help", "h", "Show this detailed help page.");
        AnsiConsole.Write(commands);
        AnsiConsole.WriteLine();

        var output = new Table().RoundedBorder().AddColumns("Option", "Description");
        output.AddRow("--json", "Print raw JSON output (no table formatting).");
        output.AddRow("--output table|json", "Explicitly choose output mode.");
        AnsiConsole.Write(output);
        AnsiConsole.WriteLine();

        var examples = new Table().RoundedBorder().AddColumns("Example", "What it does");
        examples.AddRow("manifestdex search \"elden ring\"", "Find matching games.");
        examples.AddRow("manifestdex info 730", "Get details for appId 730.");
        examples.AddRow("manifestdex get 730 --json", "Fetch keys as JSON.");
        examples.AddRow("manifestdex manifestfile 730", "Download latest manifests for appId 730.");
        examples.AddRow("manifestdex mf 730 --depots 731", "Download filtered depots.");
        examples.AddRow("manifestdex mf --manifests 731:9238 --zip", "Download specific version as zip.");
        examples.AddRow("manifestdex online-fix list --page 1", "List available online-fixes.");
        examples.AddRow("manifestdex bypass download 730", "Get download link for appId 730 bypass.");
        examples.AddRow("manifestdex usage", "Read current daily quota state.");
        examples.AddRow("manifestdex health", "Ping API health endpoint.");
        AnsiConsole.Write(examples);

        return 0;
    }
}
