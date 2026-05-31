using ManifestDeX.Cli.Application.Errors;
using ManifestDeX.Cli.Application.UseCases;
using Spectre.Console;
using Spectre.Console.Cli;

namespace ManifestDeX.Cli.Presentation.Commands;

public sealed class ManifestFileCommand : AsyncCommand<ManifestFileCommand.Settings>
{
    private readonly DownloadManifestsUseCase _useCase;

    public ManifestFileCommand(DownloadManifestsUseCase useCase)
    {
        _useCase = useCase;
    }

    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "[appId]")]
        public string? AppId { get; init; }

        [CommandOption("-d|--depots <DEPOTS>")]
        public string? Depots { get; init; }

        [CommandOption("-m|--manifests <VALUES>")]
        public string? Manifests { get; init; }

        [CommandOption("-o|--out-dir <PATH>")]
        public string? OutDir { get; init; }

        [CommandOption("-z|--zip")]
        public bool Zip { get; init; }
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Inputs Parsing & CLI Validation
            uint? appIdValue = null;
            if (!string.IsNullOrWhiteSpace(settings.AppId))
            {
                if (!uint.TryParse(settings.AppId, out var parsedAppId) || parsedAppId == 0)
                {
                    throw new CliException("appId must be a positive integer.", CliExitCode.ValidationError);
                }
                appIdValue = parsedAppId;
            }

            List<uint>? depotIdsList = null;
            if (!string.IsNullOrWhiteSpace(settings.Depots))
            {
                depotIdsList = settings.Depots.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => uint.TryParse(x.Trim(), out var val) ? val : 0)
                    .Where(val => val > 0)
                    .ToList();
            }

            List<string>? manifestsList = null;
            if (!string.IsNullOrWhiteSpace(settings.Manifests))
            {
                manifestsList = settings.Manifests.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .Where(x => !string.IsNullOrEmpty(x))
                    .ToList();
            }

            // Mutual exclusivity validations
            var hasAppId = appIdValue.HasValue;
            var hasManifests = manifestsList != null && manifestsList.Count > 0;

            if (!hasAppId && !hasManifests)
            {
                throw new CliException(
                    "You must specify either an appId or the --manifests option.\n\n" +
                    "Examples:\n" +
                    "  manifestdex manifestfile 730                             (AppID Mode)\n" +
                    "  manifestdex mf 730 --depots 731,732                      (Filtered Depots Mode)\n" +
                    "  manifestdex mf --manifests 731:8927392,732:892398 --zip  (Specific Manifests Mode)",
                    CliExitCode.ValidationError);
            }

            if (hasAppId && hasManifests)
            {
                throw new CliException(
                    "You cannot specify both an appId and the --manifests option.\n\n" +
                    "Examples:\n" +
                    "  manifestdex manifestfile 730                             (AppID Mode)\n" +
                    "  manifestdex mf --manifests 731:8927392,732:892398 --zip  (Specific Manifests Mode)",
                    CliExitCode.ValidationError);
            }

            if (depotIdsList != null && depotIdsList.Count > 0 && !hasAppId)
            {
                throw new CliException(
                    "The --depots option can only be used when an appId is provided.\n\n" +
                    "Example:\n" +
                    "  manifestdex mf 730 --depots 731,732                      (Filtered Depots Mode)",
                    CliExitCode.ValidationError);
            }

            AnsiConsole.MarkupLine("[bold deepskyblue1]ManifestDeX Downloader[/]");
            AnsiConsole.MarkupLine($"[grey]Mode:[/] {(hasAppId ? "AppID Mode" : "Specific Manifests Mode")}");
            if (hasAppId)
            {
                AnsiConsole.MarkupLine($"[grey]App ID:[/] [deepskyblue1]{appIdValue}[/]");
                if (depotIdsList != null && depotIdsList.Count > 0)
                {
                    AnsiConsole.MarkupLine($"[grey]Filtered Depots:[/] {string.Join(", ", depotIdsList)}");
                }
            }
            else
            {
                AnsiConsole.MarkupLine($"[grey]Manifests Count:[/] [deepskyblue1]{manifestsList!.Count}[/]");
            }
            AnsiConsole.MarkupLine($"[grey]Output Format:[/] {(settings.Zip ? "[yellow]ZIP Archive[/]" : "[green]Individual Manifest Files[/]")}");
            AnsiConsole.WriteLine();

            // 2. Execution with beautiful Spectre Console Progress Bars
            await AnsiConsole.Progress()
                .Columns(new ProgressColumn[]
                {
                    new TaskDescriptionColumn(),
                    new ProgressBarColumn(),
                    new PercentageColumn(),
                    new SpinnerColumn()
                })
                .StartAsync(async ctx =>
                {
                    var progressTask = ctx.AddTask("[deepskyblue1]Initializing...[/]", maxValue: 100);

                    await _useCase.ExecuteAsync(
                        appIdValue,
                        depotIdsList,
                        manifestsList,
                        settings.OutDir,
                        settings.Zip,
                        (statusText, percentage) =>
                        {
                            progressTask.Description = $"[deepskyblue1]{Markup.Escape(statusText)}[/]";
                            progressTask.Value = percentage;
                        },
                        cancellationToken);
                });

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[bold green]SUCCESS:[/] Manifest download task completed successfully!");
            return (int)CliExitCode.Success;
        }
        catch (CliException ex)
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[bold red]Error:[/] [red]{Markup.Escape(ex.Message)}[/]");
            return (int)ex.ExitCode;
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[bold red]Fatal Error:[/] [red]{Markup.Escape(ex.Message)}[/]");
            return (int)CliExitCode.UnknownError;
        }
    }
}
