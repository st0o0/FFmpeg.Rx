using FFmpeg.Rx.Events;
using R3;

namespace FFmpeg.Rx;

public static class FfmpegSessionExtensions
{
    public static Observable<double> PercentageProgress(this IFfmpegSession session, TimeSpan totalDuration) =>
        session.Progress.Select(p => Math.Min(100.0, p.TimeUs / totalDuration.TotalMicroseconds * 100.0));
}
