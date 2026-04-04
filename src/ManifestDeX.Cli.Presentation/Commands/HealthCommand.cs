using ManifestDeX.Cli.Application.Errors;
using ManifestDeX.Cli.Application.UseCases;
using ManifestDeX.Cli.Presentation.Output;
using Spectre.Console.Cli;

namespace ManifestDeX.Cli.Presentation.Commands;

public sealed class HealthCommand : AsyncCommand<HealthCommand.Settings>
{
    private readonly GetHealthUseCase _useCase;

    public HealthCommand(GetHealthUseCase useCase)
    {
        _useCase = useCase;
    }

    public sealed class Settings : BaseOutputSettings;

    protected override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        try
        {
            var health = await _useCase.ExecuteAsync(cancellationToken);
            ConsoleOutput.PrintObject(health, settings.ResolveOutputMode());
            return (int)CliExitCode.Success;
        }
        catch (CliException ex)
        {
            Console.Error.WriteLine(ex.Message);
            return (int)ex.ExitCode;
        }
    }
}


