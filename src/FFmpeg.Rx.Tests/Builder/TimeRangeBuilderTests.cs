using FFmpeg.Rx.Builder;

namespace FFmpeg.Rx.Tests.Builder;

public sealed class TimeRangeBuilderTests
{
    [Fact]
    public void Seek_sets_value()
    {
        var builder = new TimeRangeBuilder();
        builder.Seek(TimeSpan.FromSeconds(30));
        var range = builder.Build();
        Assert.Equal(TimeSpan.FromSeconds(30), range.Seek);
    }

    [Fact]
    public void Duration_sets_value()
    {
        var builder = new TimeRangeBuilder();
        builder.Duration(TimeSpan.FromSeconds(10));
        var range = builder.Build();
        Assert.Equal(TimeSpan.FromSeconds(10), range.Duration);
    }

    [Fact]
    public void StopTime_sets_value()
    {
        var builder = new TimeRangeBuilder();
        builder.StopTime(TimeSpan.FromMinutes(1));
        var range = builder.Build();
        Assert.Equal(TimeSpan.FromMinutes(1), range.StopTime);
    }

    [Fact]
    public void All_combined()
    {
        var builder = new TimeRangeBuilder();
        builder.Seek(TimeSpan.FromSeconds(5)).Duration(TimeSpan.FromSeconds(10));
        var range = builder.Build();
        Assert.Equal(TimeSpan.FromSeconds(5), range.Seek);
        Assert.Equal(TimeSpan.FromSeconds(10), range.Duration);
    }

    [Fact]
    public void Defaults_are_null()
    {
        var builder = new TimeRangeBuilder();
        var range = builder.Build();
        Assert.Null(range.Seek);
        Assert.Null(range.Duration);
        Assert.Null(range.StopTime);
    }
}
