namespace ManifestDeX.Cli.Application.Errors;

public enum CliExitCode
{
    Success = 0,
    ValidationError = 2,
    Unauthorized = 3,
    Forbidden = 4,
    RateLimited = 5,
    NetworkError = 6,
    UnknownError = 10
}
