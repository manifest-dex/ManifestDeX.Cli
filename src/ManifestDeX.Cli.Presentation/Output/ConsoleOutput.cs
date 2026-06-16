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

                if (info.OnlineFixDetails != null)
                {
                    infoTable.AddRow("Online-Fix", $"[green]Available[/] - Size: [yellow]{FormatBytes(info.OnlineFixDetails.TotalSize)}[/]");
                    infoTable.AddRow("Online-Fix Instructions", $"[yellow]{Markup.Escape(info.OnlineFixDetails.Instructions)}[/]");
                }
                else
                {
                    infoTable.AddRow("Online-Fix", "[red]Not Available[/]");
                }

                if (info.BypassDetails != null)
                {
                    var sizeStr = info.BypassDetails.FileSize.HasValue ? FormatBytes(info.BypassDetails.FileSize.Value) : "Unknown size";
                    infoTable.AddRow("Bypass", $"[green]Available[/] - Size: [yellow]{sizeStr}[/] - Info: {Markup.Escape(info.BypassDetails.AdditionalInfo)}");
                }
                else
                {
                    infoTable.AddRow("Bypass", "[red]Not Available[/]");
                }

                AnsiConsole.Write(infoTable);
                break;
            case Domain.Entities.CliPaginatedList<Domain.Entities.OnlineFixListItem> fixList:
                var fixTable = new Table().RoundedBorder().AddColumns("AppId", "Name");
                foreach (var item in fixList.Results)
                {
                    fixTable.AddRow(
                        $"[deepskyblue1]{item.AppId}[/]",
                        Markup.Escape(item.Name));
                }
                AnsiConsole.Write(fixTable);
                AnsiConsole.MarkupLine($"[grey]Page {fixList.Page} of {fixList.TotalPages} (Total: {fixList.TotalCount})[/]");
                break;
            case Domain.Entities.CliPaginatedList<Domain.Entities.BypassListItem> bypassList:
                var bypassTable = new Table().RoundedBorder().AddColumns("AppId", "Name", "Info");
                foreach (var item in bypassList.Results)
                {
                    bypassTable.AddRow(
                        $"[deepskyblue1]{item.AppId}[/]",
                        Markup.Escape(item.Name),
                        Markup.Escape(item.AdditionalInfo));
                }
                AnsiConsole.Write(bypassTable);
                AnsiConsole.MarkupLine($"[grey]Page {bypassList.Page} of {bypassList.TotalPages} (Total: {bypassList.TotalCount})[/]");
                break;
            case Domain.Entities.DownloadLink link:
                AnsiConsole.MarkupLine($"[green]Download link generated successfully! (Expires in 30 minutes, single-use)[/]");
                AnsiConsole.MarkupLine($"Link: [deepskyblue1]{Markup.Escape(link.Url)}[/]");
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

    private static string FormatBytes(long bytes)
    {
        if (bytes <= 0) return "0 B";
        string[] suffix = { "B", "KB", "MB", "GB", "TB" };
        int i = 0;
        double dblSByte = bytes;
        while (dblSByte >= 1024 && i < suffix.Length - 1)
        {
            dblSByte /= 1024.0;
            i++;
        }
        return $"{dblSByte:0.##} {suffix[i]}";
    }

    public static void PrintText(string text) => AnsiConsole.MarkupLine(Markup.Escape(text));
}
