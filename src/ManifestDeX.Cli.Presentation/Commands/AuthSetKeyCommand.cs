using ManifestDeX.Cli.Application.Errors;
using ManifestDeX.Cli.Application.UseCases;
using Spectre.Console.Cli;

namespace ManifestDeX.Cli.Presentation.Commands;

public sealed class AuthSetKeyCommand : AsyncCommand<AuthSetKeyCommand.Settings>
{
    private readonly SetApiKeyUseCase _useCase;

    public AuthSetKeyCommand(SetApiKeyUseCase useCase)
    {
        _useCase = useCase;
    }

    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<key>")]
        public string Key { get; init; } = string.Empty;
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        try
        {
            await _useCase.ExecuteAsync(settings.Key, cancellationToken);
            Console.WriteLine("API key saved.");
            return (int)CliExitCode.Success;
        }
        catch (CliException ex)
        {
            Console.Error.WriteLine(ex.Message);
            return (int)ex.ExitCode;
        }
    }
}


