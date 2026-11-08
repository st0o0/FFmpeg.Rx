using FFmpeg.Rx.Filters;

namespace FFmpeg.Rx.Tests.Builder;

public sealed class AudioFilterBuilderTests
{
    [Fact]
    public void Volume_factor()
    {
        var builder = new AudioFilterBuilder();
        builder.Volume(1.5);
        var filter = Assert.IsType<VolumeFilter>(Assert.Single(builder.Filters));
        Assert.Equal("volume=1.5", filter.Render());
    }

    [Fact]
    public void Volume_expression()
    {
        var builder = new AudioFilterBuilder();
        builder.Volume("0.5*between(t,10,20)+1.0*not(between(t,10,20))");
        var filter = Assert.IsType<VolumeExprFilter>(Assert.Single(builder.Filters));
        Assert.Contains("volume=", filter.Render());
    }

    [Fact]
    public void LoudNorm_default()
    {
        var builder = new AudioFilterBuilder();
        builder.LoudNorm();
        var filter = Assert.IsType<LoudNormFilter>(Assert.Single(builder.Filters));
        Assert.Equal("loudnorm", filter.Render());
    }

    [Fact]
    public void LoudNorm_with_params()
    {
        var builder = new AudioFilterBuilder();
        builder.LoudNorm(i: -24, tp: -2, lra: 7);
        var filter = Assert.IsType<LoudNormFilter>(Assert.Single(builder.Filters));
        var rendered = filter.Render();
        Assert.Contains("I=-24", rendered);
        Assert.Contains("TP=-2", rendered);
        Assert.Contains("LRA=7", rendered);
    }

    [Fact]
    public void AResample()
    {
        var builder = new AudioFilterBuilder();
        builder.AResample(44100);
        var filter = Assert.IsType<AResampleFilter>(Assert.Single(builder.Filters));
        Assert.Equal("aresample=44100", filter.Render());
    }

    [Fact]
    public void SilenceDetect()
    {
        var builder = new AudioFilterBuilder();
        builder.SilenceDetect(-30, 2);
        var filter = Assert.IsType<SilenceDetectFilter>(Assert.Single(builder.Filters));
        Assert.Contains("silencedetect=n=-30dB:d=2", filter.Render());
    }

    [Fact]
    public void Custom()
    {
        var builder = new AudioFilterBuilder();
        builder.Custom("aecho", "0.8:0.88:60:0.4");
        var filter = Assert.IsType<CustomFilter>(Assert.Single(builder.Filters));
        Assert.Equal("aecho=0.8:0.88:60:0.4", filter.Render());
    }

    [Fact]
    public void Multiple_filters()
    {
        var builder = new AudioFilterBuilder();
        builder.Volume(2.0).LoudNorm().AResample(48000);
        Assert.Equal(3, builder.Filters.Count);
    }
}
