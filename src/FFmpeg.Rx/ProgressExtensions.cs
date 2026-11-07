using FFmpeg.Rx.Events;
using R3;

namespace FFmpeg.Rx;

public static class ProgressExtensions
{
    public static double AsPercentage(this ProgressEvent progress, TimeSpan totalDuration)
    {
        if (totalDuration <= TimeSpan.Zero)
        {
            return 0.0;
        }

        var currentSeconds = progress.TimeUs / 1_000_000.0;
        var totalSeconds = totalDuration.TotalSeconds;
        return Math.Clamp(currentSeconds / totalSeconds * 100.0, 0.0, 100.0);
    }

    public static Observable<PercentageProgressEvent> WithPercentage(
        this Observable<ProgressEvent> source, TimeSpan totalDuration)
    {
        return source.Select(p => new PercentageProgressEvent(p.AsPercentage(totalDuration), p));
    }
}
