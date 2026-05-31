namespace ManifestDeX.Cli.Domain.Entities;

public sealed record DownloadQueueResponse(
    bool Success,
    string? TaskId,
    string? Status,
    string? ProgressText,
    double ProgressPercentage,
    string? DownloadUrl,
    string? Error
);
