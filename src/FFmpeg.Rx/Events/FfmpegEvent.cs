namespace FFmpeg.Rx.Events;

public abstract record FfmpegEvent;

public sealed record StartedEvent(int ProcessId, string Arguments) : FfmpegEvent;

public sealed record ProgressEvent(long BytesWritten, long TimeUs, double Speed) : FfmpegEvent;

public sealed record LogEvent(string Line, LogLevel Level) : FfmpegEvent;

public sealed record CompletedEvent(int ExitCode, TimeSpan Duration) : FfmpegEvent;

public sealed record FailedEvent(int ExitCode, string Error, FailureKind Kind) : FfmpegEvent;
