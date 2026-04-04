namespace ManifestDeX.Cli.Domain.Entities;

public sealed record GameInfo(uint AppId, string Name, string HeaderImageUrl, int TotalDecryptionKeys, IReadOnlyList<uint> DepotIds);
