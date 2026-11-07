namespace FFmpeg.Rx.Models;

public sealed record FfmpegResult(
    bool Success,
    int ExitCode,
    string? Error,
    TimeSpan Duration,
    FailureKind? FailureKind = null);
