namespace FFmpeg.Rx.Models;

public sealed record TeeOutput(TeeTarget[] Targets);

public sealed record TeeTarget(
    string Path,
    string? Format = null,
    string? Select = null,
    string? BitstreamFilter = null,
    string? MovFlags = null,
    bool IgnoreFailure = false);
