namespace ManifestDeX.Cli.Domain.Entities;

public sealed record BypassListItem(
    uint AppId,
    string Name,
    string AdditionalInfo
);
