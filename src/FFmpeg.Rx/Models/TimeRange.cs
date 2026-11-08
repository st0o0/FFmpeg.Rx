namespace FFmpeg.Rx.Models;

public sealed record TimeRange(
    TimeSpan? Seek = null,
    TimeSpan? Duration = null,
    TimeSpan? StopTime = null);
