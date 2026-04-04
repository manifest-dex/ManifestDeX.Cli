using ManifestDeX.Cli.Application.Errors;
using ManifestDeX.Cli.Application.UseCases;
using ManifestDeX.Cli.Presentation.Output;
using Spectre.Console.Cli;

namespace ManifestDeX.Cli.Presentation.Commands;

public sealed class SearchCommand : AsyncCommand<SearchCommand.Settings>
{
    private readonly SearchGameUseCase _useCase;

    public SearchCommand(SearchGameUseCase useCase)
    {
        _useCase = useCase;
    }

    public sealed class Settings : BaseOutputSettings
    {
        [CommandArgument(0, "<query>")]
        public string Query { get; init; } = string.Empty;
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        try
        {
            var results = await _useCase.ExecuteAsync(settings.Query, cancellationToken);
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


