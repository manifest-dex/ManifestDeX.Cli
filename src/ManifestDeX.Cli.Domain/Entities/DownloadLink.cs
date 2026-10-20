namespace ManifestDeX.Cli.Domain.Entities;

public sealed record DownloadLink(string Url, bool? IsPasswordProtected = null, string? ArchivePassword = null);
