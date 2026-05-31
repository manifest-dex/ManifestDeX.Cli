namespace ManifestDeX.Cli.Domain.Entities;

public sealed record AvailableManifest(uint DepotId, ulong ManifestId, ulong SizeBytes);
