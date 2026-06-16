namespace ManifestDeX.Cli.Domain.Entities;

public sealed record OnlineFixDetails(long TotalSize, bool IsSplitArchive, string Instructions);

public sealed record BypassDetails(long? FileSize, string AdditionalInfo, DateTime LastUpdated);

public sealed record GameInfo(
    uint AppId, 
    string Name, 
    string HeaderImageUrl, 
    int TotalDecryptionKeys, 
    IReadOnlyList<uint> DepotIds,
    OnlineFixDetails? OnlineFixDetails = null,
    BypassDetails? BypassDetails = null
);
