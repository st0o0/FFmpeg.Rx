using FFmpeg.Rx.FilterGraph;
using FFmpeg.Rx.Filters;

namespace FFmpeg.Rx.Tests.Builder;

public sealed class FilterChainBuilderTests
{
    [Fact]
    public void Single_scale()
    {
        var builder = new FilterChainBuilder();
        builder.Scale(1920, 1080);
        Assert.Equal("scale=1920:1080", builder.Render());
    }

    [Fact]
    public void Scale_dimension()
    {
        var builder = new FilterChainBuilder();
        builder.Scale(720, ScaleDimension.Height);
        Assert.Equal("scale=-2:720", builder.Render());
    }

    [Fact]
    public void Crop()
    {
        var builder = new FilterChainBuilder();
        builder.Crop(640, 480, 0, 0);
        Assert.Equal("crop=640:480:0:0", builder.Render());
    }

    [Fact]
    public void Chain_multiple()
    {
        var builder = new FilterChainBuilder();
        builder.Scale(1920, 1080).HorizontalFlip().Yadif();
        Assert.Equal("scale=1920:1080,hflip,yadif", builder.Render());
    }

    [Fact]
    public void Fade()
    {
        var builder = new FilterChainBuilder();
        builder.Fade(FadeType.In, 0, 30);
        Assert.Equal("fade=t=in:s=0:n=30", builder.Render());
    }

    [Fact]
    public void VerticalFlip()
    {
        var builder = new FilterChainBuilder();
        builder.VerticalFlip();
        Assert.Equal("vflip", builder.Render());
    }

    [Fact]
    public void BurnSubtitles()
    {
        var builder = new FilterChainBuilder();
        builder.BurnSubtitles("subs.srt");
        Assert.Equal("subtitles=subs.srt", builder.Render());
    }

    [Fact]
    public void Custom()
    {
        var builder = new FilterChainBuilder();
        builder.Custom("eq", "brightness=0.06");
        Assert.Equal("eq=brightness=0.06", builder.Render());
    }

    [Fact]
    public void Custom_without_args()
    {
        var builder = new FilterChainBuilder();
        builder.Custom("null");
        Assert.Equal("null", builder.Render());
    }
}
