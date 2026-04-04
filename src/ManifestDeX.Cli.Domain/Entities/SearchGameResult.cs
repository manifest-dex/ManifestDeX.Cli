namespace ManifestDeX.Cli.Domain.Entities;

public sealed record SearchGameResult(uint AppId, string Name, int AvailableDecryptionKeys);
