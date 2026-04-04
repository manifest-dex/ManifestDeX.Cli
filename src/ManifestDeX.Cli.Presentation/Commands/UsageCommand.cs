using ManifestDeX.Cli.Application.Errors;
using ManifestDeX.Cli.Application.UseCases;
using ManifestDeX.Cli.Presentation.Output;
using Spectre.Console.Cli;

namespace ManifestDeX.Cli.Presentation.Commands;

public sealed class UsageCommand : AsyncCommand<UsageCommand.Settings>
{
    private readonly GetUsageUseCase _useCase;

    public UsageCommand(GetUsageUseCase useCase)
    {
        _useCase = useCase;
    }

    public sealed class Settings : BaseOutputSettings;

    protected override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        try
        {
            var usage = await _useCase.ExecuteAsync(cancellationToken);
            ConsoleOutput.PrintObject(usage, settings.ResolveOutputMode());
            return (int)CliExitCode.Success;
        }
        catch (CliException ex)
        {
            Console.Error.WriteLine(ex.Message);
            return (int)ex.ExitCode;
        }
    }
}


