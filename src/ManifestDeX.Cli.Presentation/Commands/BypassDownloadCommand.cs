using ManifestDeX.Cli.Application.Errors;
using ManifestDeX.Cli.Application.UseCases;
using ManifestDeX.Cli.Presentation.Output;
using Spectre.Console.Cli;

namespace ManifestDeX.Cli.Presentation.Commands;

public sealed class BypassDownloadCommand : AsyncCommand<BypassDownloadCommand.Settings>
{
    private readonly GetBypassDownloadLinkUseCase _useCase;

    public BypassDownloadCommand(GetBypassDownloadLinkUseCase useCase)
    {
        _useCase = useCase;
    }

    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<appId>")]
        public string AppId { get; init; } = string.Empty;
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        try
        {
            if (!uint.TryParse(settings.AppId, out var appId) || appId == 0)
            {
                throw new CliException("appId must be a positive integer.", CliExitCode.ValidationError);
            }

            var link = await _useCase.ExecuteAsync(appId, cancellationToken);
            ConsoleOutput.PrintObject(link, Parsing.OutputMode.Table);
            return (int)CliExitCode.Success;
        }
        catch (CliException ex)
        {
            Console.Error.WriteLine(ex.Message);
            return (int)ex.ExitCode;
        }
    }
}
