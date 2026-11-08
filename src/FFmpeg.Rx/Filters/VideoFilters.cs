using System.Globalization;

namespace FFmpeg.Rx.Filters;

public sealed record ScaleFilter(int Width, int Height) : IVideoFilter
{
    public string Render() => $"scale={Width}:{Height}";
}

public sealed record ScaleDimensionFilter(int Value, ScaleDimension Dimension) : IVideoFilter
{
    public string Render()
    {
        return Dimension == ScaleDimension.Width
            ? $"scale={Value}:-2"
            : $"scale=-2:{Value}";
    }
}

public enum ScaleDimension
{
    Width,
    Height
}

public sealed record CropFilter(int Width, int Height, int X, int Y) : IVideoFilter
{
    public string Render() => $"crop={Width}:{Height}:{X}:{Y}";
}

public sealed record CropExprFilter(string Expression) : IVideoFilter
{
    public string Render() => $"crop={Expression}";
}

public sealed record HFlipFilter : IVideoFilter
{
    public string Render() => "hflip";
}

public sealed record VFlipFilter : IVideoFilter
{
    public string Render() => "vflip";
}

public sealed record TransposeFilter(TransposeMode Mode) : IVideoFilter
{
    public string Render() => $"transpose={(int)Mode}";
}

public enum TransposeMode
{
    CounterClockFlip = 0,
    Clock = 1,
    CounterClock = 2,
    ClockFlip = 3
}

public sealed record FadeFilter : IVideoFilter
{
    public FadeType Type { get; init; }
    public int? StartFrame { get; init; }
    public int? Frames { get; init; }
    public TimeSpan? Start { get; init; }
    public TimeSpan? Duration { get; init; }

    public FadeFilter(FadeType type, int startFrame, int frames)
    {
        Type = type;
        StartFrame = startFrame;
        Frames = frames;
    }

    public FadeFilter(FadeType type, TimeSpan start, TimeSpan duration)
    {
        Type = type;
        Start = start;
        Duration = duration;
    }

    public string Render()
    {
        var t = Type == FadeType.In ? "in" : "out";
        if (StartFrame.HasValue)
        {
            return $"fade=t={t}:s={StartFrame}:n={Frames}";
        }
        var st = Start!.Value.TotalSeconds.ToString("F3", CultureInfo.InvariantCulture);
        var d = Duration!.Value.TotalSeconds.ToString("F3", CultureInfo.InvariantCulture);
        return $"fade=t={t}:st={st}:d={d}";
    }
}

public enum FadeType
{
    In,
    Out
}

public sealed record PadFilter(int Width, int Height, int X, int Y, string Color) : IVideoFilter
{
    public string Render() => $"pad={Width}:{Height}:{X}:{Y}:{Color}";
}

public sealed record YadifFilter : IVideoFilter
{
    public string Render() => "yadif";
}
