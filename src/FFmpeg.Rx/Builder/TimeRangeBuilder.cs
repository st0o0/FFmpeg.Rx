using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.Builder;

public sealed class TimeRangeBuilder
{
    private TimeSpan? _seek;
    private TimeSpan? _duration;
    private TimeSpan? _stopTime;

    public TimeRangeBuilder Seek(TimeSpan position)
    {
        _seek = position;
        return this;
    }

    public TimeRangeBuilder Duration(TimeSpan length)
    {
        _duration = length;
        return this;
    }

    public TimeRangeBuilder StopTime(TimeSpan position)
    {
        _stopTime = position;
        return this;
    }

    internal TimeRange Build()
    {
        return new TimeRange(_seek, _duration, _stopTime);
    }
}
