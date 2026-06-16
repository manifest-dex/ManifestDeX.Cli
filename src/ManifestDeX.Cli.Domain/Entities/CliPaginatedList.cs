namespace ManifestDeX.Cli.Domain.Entities;

public sealed record CliPaginatedList<T>(
    IReadOnlyList<T> Results,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages
);
