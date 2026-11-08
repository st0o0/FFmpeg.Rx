using FFmpeg.Rx.Events;

namespace FFmpeg.Rx.Tests;

public sealed class ExtensionTests
{
    [Fact]
    public void EmitProgress_without_duration_yields_one_event()
    {
        var progress = new ProgressEvent(1024, 2_500_000, 1.5);
        var events = FfmpegSession.EmitProgress(progress, null).ToList();

        Assert.Single(events);
        Assert.Same(progress, events[0]);
    }

    [Fact]
    public void EmitProgress_with_duration_yields_progress_and_percentage()
    {
        var progress = new ProgressEvent(1024, 2_500_000, 1.5);
        var duration = TimeSpan.FromSeconds(5);
        var events = FfmpegSession.EmitProgress(progress, duration).ToList();

        Assert.Equal(2, events.Count);
        Assert.Same(progress, events[0]);
        var percentage = Assert.IsType<PercentageProgressEvent>(events[1]);
        Assert.Equal(50.0, percentage.Percent, 0.1);
    }

    [Fact]
    public void EmitProgress_clamps_at_100_percent()
    {
        var progress = new ProgressEvent(1024, 10_000_000, 1.0);
        var duration = TimeSpan.FromSeconds(5);
        var events = FfmpegSession.EmitProgress(progress, duration).ToList();

        var percentage = Assert.IsType<PercentageProgressEvent>(events[1]);
        Assert.Equal(100.0, percentage.Percent);
    }

    [Fact]
    public void EmitProgress_with_zero_duration_yields_one_event()
    {
        var progress = new ProgressEvent(1024, 2_500_000, 1.5);
        var events = FfmpegSession.EmitProgress(progress, TimeSpan.Zero).ToList();

        Assert.Single(events);
    }

    [Fact]
    public void ClassifyLogLevel_error()
    {
        Assert.Equal(LogLevel.Error, FfmpegSession.ClassifyLogLevel("[error] something failed"));
    }

    [Fact]
    public void ClassifyLogLevel_warning()
    {
        Assert.Equal(LogLevel.Warning, FfmpegSession.ClassifyLogLevel("[warning] deprecated codec"));
    }

    [Fact]
    public void ClassifyLogLevel_fatal()
    {
        Assert.Equal(LogLevel.Fatal, FfmpegSession.ClassifyLogLevel("[fatal] out of memory"));
    }

    [Fact]
    public void ClassifyLogLevel_info()
    {
        Assert.Equal(LogLevel.Info, FfmpegSession.ClassifyLogLevel("Stream mapping:"));
    }
}
