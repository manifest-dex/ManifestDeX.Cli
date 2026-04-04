using System.Text.Json;
using Spectre.Console;

namespace ManifestDeX.Cli.Presentation.Output;

public static class ConsoleOutput
{
    public static void PrintObject(object payload, Parsing.OutputMode outputMode)
    {
        if (outputMode == Parsing.OutputMode.Json)
        {
            var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true });
            Console.WriteLine(json);
            return;
        }

        switch (payload)
        {
            case IEnumerable<Domain.Entities.SearchGameResult> results:
                var searchTable = new Table().RoundedBorder().AddColumns("AppId", "Keys", "Name");
                foreach (var item in results)
                {
                    searchTable.AddRow(
                        $"[deepskyblue1]{item.AppId}[/]",
                        $"[green]{item.AvailableDecryptionKeys}[/]",
                        Markup.Escape(item.Name));
                }
                AnsiConsole.Write(searchTable);
                break;
            case Domain.Entities.GameInfo info:
                var depots = info.DepotIds.Count == 0 ? "-" : string.Join(", ", info.DepotIds);
                var infoTable = new Table().RoundedBorder().AddColumns("Field", "Value");
                infoTable.AddRow("AppId", $"[deepskyblue1]{info.AppId}[/]");
                infoTable.AddRow("Name", Markup.Escape(info.Name));
                infoTable.AddRow("Header image URL", string.IsNullOrWhiteSpace(info.HeaderImageUrl) ? "-" : Markup.Escape(info.HeaderImageUrl));
                infoTable.AddRow("Total decryption keys", $"[green]{info.TotalDecryptionKeys}[/]");
                infoTable.AddRow("Depots", Markup.Escape(depots));
                AnsiConsole.Write(infoTable);
                break;
            case IEnumerable<Domain.Entities.DepotKey> keys:
                var keyTable = new Table().RoundedBorder().AddColumns("DepotId", "Key");
                foreach (var key in keys)
                {
                    keyTable.AddRow(
                        $"[deepskyblue1]{key.DepotId}[/]",
                        $"[grey]{Markup.Escape(key.Key)}[/]");
                }
                AnsiConsole.Write(keyTable);
                break;
            case Domain.Entities.UsageSnapshot usage:
                var usageTable = new Table().RoundedBorder().AddColumns("Metric", "Value");
                usageTable.AddRow("Daily limit", $"[green]{usage.DailyLimit}[/]");
                usageTable.AddRow("Used today", $"[yellow]{usage.UsedToday}[/]");
                usageTable.AddRow("Remaining today", $"[green]{usage.RemainingToday}[/]");
                usageTable.AddRow("Reset at (UTC)", $"[grey]{usage.ResetAtUtc:O}[/]");
                AnsiConsole.Write(usageTable);
                break;
            case Domain.Entities.HealthSnapshot health:
                AnsiConsole.Write(
                    new Panel(new Markup($"[green]Status[/]: {Markup.Escape(health.Status)}\n[grey]Timestamp (UTC)[/]: {health.TimestampUtc:O}"))
                        .Header("[blue]Health[/]")
                        .Border(BoxBorder.Rounded));
                break;
            default:
                AnsiConsole.MarkupLine(Markup.Escape(payload.ToString() ?? string.Empty));
                break;
        }
    }

    public static void PrintText(string text) => AnsiConsole.MarkupLine(Markup.Escape(text));
}
