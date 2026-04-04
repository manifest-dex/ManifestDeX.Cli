namespace ManifestDeX.Cli.Presentation.Parsing;

public enum OutputMode
{
    Table,
    Json
}

public sealed class ParsedCommand
{
    public string Name { get; init; } = string.Empty;
    public IReadOnlyList<string> Arguments { get; init; } = [];
    public OutputMode OutputMode { get; init; } = OutputMode.Table;
}

public static class CommandParser
{
    public static ParsedCommand Parse(string[] args)
    {
        if (args.Length == 0)
        {
            return new ParsedCommand { Name = "help" };
        }

        var outputMode = OutputMode.Table;
        var remaining = new List<string>();

        for (var i = 0; i < args.Length; i++)
        {
            var current = args[i];
            if (current.Equals("--json", StringComparison.OrdinalIgnoreCase))
            {
                outputMode = OutputMode.Json;
                continue;
            }

            if (current.Equals("--output", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length)
            {
                var value = args[++i];
                outputMode = value.Equals("json", StringComparison.OrdinalIgnoreCase) ? OutputMode.Json : OutputMode.Table;
                continue;
            }

            remaining.Add(current);
        }

        if (remaining.Count == 0)
        {
            return new ParsedCommand { Name = "help", OutputMode = outputMode };
        }

        if (remaining.Count >= 2 && remaining[0].Equals("auth", StringComparison.OrdinalIgnoreCase))
        {
            return new ParsedCommand
            {
                Name = $"auth:{remaining[1].ToLowerInvariant()}",
                Arguments = remaining.Skip(2).ToArray(),
                OutputMode = outputMode
            };
        }

        return new ParsedCommand
        {
            Name = remaining[0].ToLowerInvariant(),
            Arguments = remaining.Skip(1).ToArray(),
            OutputMode = outputMode
        };
    }
}
