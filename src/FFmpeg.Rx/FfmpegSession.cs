using FFmpeg.Rx.Events;
using FFmpeg.Rx.Internal;
using FFmpeg.Rx.Models;
using R3;

namespace FFmpeg.Rx;

public sealed class FfmpegSession : IAsyncDisposable
{
    private readonly ProcessRunner _runner;
    private readonly Subject<FfmpegEvent> _subject = new();
    private int _started;

    private FfmpegSession(FfmpegPipeline pipeline)
    {
        _runner = new ProcessRunner(pipeline, _subject);
    }

    public Observable<FfmpegEvent> Events => _subject.AsObservable();

    public Observable<ProgressEvent> Progress => Events.OfType<FfmpegEvent, ProgressEvent>();

    public Observable<LogEvent> Log => Events.OfType<FfmpegEvent, LogEvent>();

    public async Task<FfmpegResult> RunAsync(CancellationToken ct = default)
    {
        if (Interlocked.Exchange(ref _started, 1) != 0)
        {
            throw new InvalidOperationException(
                "This session has already been run. Build a new session from the pipeline.");
        }

        return await _runner.RunAsync(ct).ConfigureAwait(false);
    }

    public static FfmpegSession Create(FfmpegPipeline pipeline) => new(pipeline);

    public ValueTask DisposeAsync()
    {
        _runner.Kill();
        _subject.Dispose();
        return ValueTask.CompletedTask;
    }
}
