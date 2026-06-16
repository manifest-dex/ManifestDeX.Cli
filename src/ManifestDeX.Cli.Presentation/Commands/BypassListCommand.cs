using ManifestDeX.Cli.Application.Errors;
using ManifestDeX.Cli.Application.UseCases;
using ManifestDeX.Cli.Presentation.Output;
using Spectre.Console.Cli;

namespace ManifestDeX.Cli.Presentation.Commands;

public sealed class BypassListCommand : AsyncCommand<BypassListCommand.Settings>
{
    private readonly ListBypassesUseCase _useCase;

    public BypassListCommand(ListBypassesUseCase useCase)
    {
        _useCase = useCase;
    }

    public sealed class Settings : BaseOutputSettings
    {
        [CommandOption("-p|--page <PAGE>")]
        public int? Page { get; init; }

        [CommandOption("-s|--page-size <PAGE_SIZE>")]
        public int? PageSize { get; init; }
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        try
        {
            var page = settings.Page ?? 1;
            var pageSize = settings.PageSize ?? 10;

            var results = await _useCase.ExecuteAsync(page, pageSize, cancellationToken);
            ConsoleOutput.PrintObject(results, settings.ResolveOutputMode());
            return (int)CliExitCode.Success;
        }
        catch (CliException ex)
        {
            Console.Error.WriteLine(ex.Message);
            return (int)ex.ExitCode;
        }
    }
}
