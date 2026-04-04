using ManifestDeX.Cli.Application.Errors;
using ManifestDeX.Cli.Application.UseCases;
using ManifestDeX.Cli.Presentation.Output;
using Spectre.Console.Cli;

namespace ManifestDeX.Cli.Presentation.Commands;

public sealed class AuthStatusCommand : AsyncCommand<AuthStatusCommand.Settings>
{
    private readonly GetAuthStatusUseCase _useCase;

    public AuthStatusCommand(GetAuthStatusUseCase useCase)
    {
        _useCase = useCase;
    }

    public sealed class Settings : BaseOutputSettings;

    protected override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        try
        {
            var status = await _useCase.ExecuteAsync(cancellationToken);
            if (!status.hasKey)
            {
                Console.WriteLine("API key is not configured.");
                return (int)CliExitCode.ValidationError;
            }

            Console.WriteLine("API key is configured and valid.");
            if (status.usage is not null)
            {
                ConsoleOutput.PrintObject(status.usage, settings.ResolveOutputMode());
            }

            return (int)CliExitCode.Success;
        }
        catch (CliException ex)
        {
            Console.Error.WriteLine(ex.Message);
            return (int)ex.ExitCode;
        }
    }
}


