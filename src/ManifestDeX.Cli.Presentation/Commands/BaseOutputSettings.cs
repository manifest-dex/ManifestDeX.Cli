using Spectre.Console.Cli;

namespace ManifestDeX.Cli.Presentation.Commands;

public abstract class BaseOutputSettings : CommandSettings
{
    [CommandOption("--json")]
    public bool Json { get; init; }

    [CommandOption("--output <MODE>")]
    public string? Output { get; init; }

    public Parsing.OutputMode ResolveOutputMode()
    {
        if (Json)
        {
            return Parsing.OutputMode.Json;
        }

        if (!string.IsNullOrWhiteSpace(Output) && Output.Equals("json", StringComparison.OrdinalIgnoreCase))
        {
            return Parsing.OutputMode.Json;
        }

        return Parsing.OutputMode.Table;
    }
}


