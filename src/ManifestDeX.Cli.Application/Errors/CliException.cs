namespace ManifestDeX.Cli.Application.Errors;

public sealed class CliException : Exception
{
    public CliExitCode ExitCode { get; }

    public CliException(string message, CliExitCode exitCode, Exception? inner = null)
        : base(message, inner)
    {
        ExitCode = exitCode;
    }
}
