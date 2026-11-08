using FFmpeg.Rx.Builder;
using FFmpeg.Rx.Codecs;

namespace FFmpeg.Rx.Tests.Builder;

public sealed class VideoEncodingBuilderTests
{
    [Fact]
    public void Quality_sets_crf()
    {
        var builder = new VideoEncodingBuilder();
        builder.Quality(Quality.Crf(23));
        var opts = builder.Build();
        var cq = Assert.IsType<ConstantQuality>(opts.Quality);
        Assert.Equal(23, cq.Crf);
    }

    [Fact]
    public void Quality_sets_bitrate()
    {
        var builder = new VideoEncodingBuilder();
        builder.Quality(Quality.Bitrate(Bitrate.Mbps(4)));
        var opts = builder.Build();
        var cb = Assert.IsType<ConstantBitrate>(opts.Quality);
        Assert.Equal(4_000_000, cb.Rate.BitsPerSecond);
    }

    [Fact]
    public void Preset_sets_value()
    {
        var builder = new VideoEncodingBuilder();
        builder.Preset(Preset.Slow);
        var opts = builder.Build();
        Assert.Equal(Preset.Slow, opts.Preset);
    }

    [Fact]
    public void Profile_sets_value()
    {
        var builder = new VideoEncodingBuilder();
        builder.Profile(VideoProfile.High);
        var opts = builder.Build();
        Assert.Equal(VideoProfile.High, opts.Profile);
    }

    [Fact]
    public void Tune_sets_value()
    {
        var builder = new VideoEncodingBuilder();
        builder.Tune(Tune.Film);
        var opts = builder.Build();
        Assert.Equal(Tune.Film, opts.Tune);
    }

    [Fact]
    public void PixelFormat_sets_value()
    {
        var builder = new VideoEncodingBuilder();
        builder.PixelFormat(Codecs.PixelFormat.Yuv420p);
        var opts = builder.Build();
        Assert.Equal(Codecs.PixelFormat.Yuv420p, opts.PixelFormat);
    }

    [Fact]
    public void Framerate_sets_value()
    {
        var builder = new VideoEncodingBuilder();
        builder.Framerate(Framerate.Fps(30));
        var opts = builder.Build();
        Assert.Equal(Framerate.Fps(30), opts.Framerate);
    }

    [Fact]
    public void All_options_combined()
    {
        var builder = new VideoEncodingBuilder();
        builder
            .Quality(Quality.Crf(18))
            .Preset(Preset.VerySlow)
            .Profile(VideoProfile.High)
            .Tune(Tune.Film)
            .PixelFormat(Codecs.PixelFormat.Yuv420p)
            .Framerate(Framerate.Ntsc30);

        var opts = builder.Build();
        Assert.IsType<ConstantQuality>(opts.Quality);
        Assert.Equal(Preset.VerySlow, opts.Preset);
        Assert.Equal(VideoProfile.High, opts.Profile);
        Assert.Equal(Tune.Film, opts.Tune);
        Assert.Equal(Codecs.PixelFormat.Yuv420p, opts.PixelFormat);
        Assert.Equal(Framerate.Ntsc30, opts.Framerate);
    }

    [Fact]
    public void Defaults_are_null()
    {
        var builder = new VideoEncodingBuilder();
        var opts = builder.Build();
        Assert.Null(opts.Quality);
        Assert.Null(opts.Preset);
        Assert.Null(opts.Profile);
        Assert.Null(opts.Tune);
        Assert.Null(opts.PixelFormat);
        Assert.Null(opts.Framerate);
    }
}
