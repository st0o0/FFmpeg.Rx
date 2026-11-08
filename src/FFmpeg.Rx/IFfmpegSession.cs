using FFmpeg.Rx.Events;
using FFmpeg.Rx.Models;
using R3;

namespace FFmpeg.Rx;

public interface IFfmpegSession : IAsyncDisposable
{
    Observable<FfmpegEvent> Events { get; }
    Observable<ProgressEvent> Progress { get; }
    Observable<LogEvent> Log { get; }
    Task<FfmpegResult> RunAsync(CancellationToken ct = default);
}
