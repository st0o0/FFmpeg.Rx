using FFmpeg.Rx.Events;

namespace FFmpeg.Rx.Tests;

public sealed class PercentageProgressTests
{
    [Fact]
    public void AsPercentage_calculates_correctly()
    {
        var progress = new ProgressEvent(0, 30_000_000, 1.0);
        var pct = progress.AsPercentage(TimeSpan.FromSeconds(60));
        Assert.Equal(50.0, pct, precision: 1);
    }

    [Fact]
    public void AsPercentage_zero_duration_returns_zero()
    {
        var progress = new ProgressEvent(0, 30_000_000, 1.0);
        var pct = progress.AsPercentage(TimeSpan.Zero);
        Assert.Equal(0.0, pct);
    }

    [Fact]
    public void AsPercentage_clamps_to_100()
    {
        var progress = new ProgressEvent(0, 120_000_000, 1.0);
        var pct = progress.AsPercentage(TimeSpan.FromSeconds(60));
        Assert.Equal(100.0, pct);
    }

    [Fact]
    public void AsPercentage_at_start()
    {
        var progress = new ProgressEvent(0, 0, 0.0);
        var pct = progress.AsPercentage(TimeSpan.FromSeconds(60));
        Assert.Equal(0.0, pct);
    }

    [Fact]
    public void AsPercentage_at_end()
    {
        var progress = new ProgressEvent(0, 60_000_000, 1.0);
        var pct = progress.AsPercentage(TimeSpan.FromSeconds(60));
        Assert.Equal(100.0, pct);
    }
}
