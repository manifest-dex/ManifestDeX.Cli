namespace ManifestDeX.Cli.Domain.Entities;

public sealed record UsageSnapshot(int DailyLimit, int UsedToday, int RemainingToday, DateTime ResetAtUtc);
