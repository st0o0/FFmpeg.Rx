using FFmpeg.Rx.Builder;
using FFmpeg.Rx.Codecs;

namespace FFmpeg.Rx.Tests.Builder;

public sealed class AudioEncodingBuilderTests
{
    [Fact]
    public void Bitrate_sets_value()
    {
        var builder = new AudioEncodingBuilder();
        builder.Bitrate(Bitrate.Kbps(192));
        var opts = builder.Build();
        Assert.Equal(Bitrate.Kbps(192), opts.Bitrate);
    }

    [Fact]
    public void SampleRate_sets_value()
    {
        var builder = new AudioEncodingBuilder();
        builder.SampleRate(44100);
        var opts = builder.Build();
        Assert.Equal(44100, opts.SampleRate);
    }

    [Fact]
    public void Channels_sets_value()
    {
        var builder = new AudioEncodingBuilder();
        builder.Channels(2);
        var opts = builder.Build();
        Assert.Equal(2, opts.Channels);
    }

    [Fact]
    public void All_options_combined()
    {
        var builder = new AudioEncodingBuilder();
        builder.Bitrate(Bitrate.Kbps(128)).SampleRate(48000).Channels(1);
        var opts = builder.Build();
        Assert.Equal(Bitrate.Kbps(128), opts.Bitrate);
        Assert.Equal(48000, opts.SampleRate);
        Assert.Equal(1, opts.Channels);
    }

    [Fact]
    public void Defaults_are_null()
    {
        var builder = new AudioEncodingBuilder();
        var opts = builder.Build();
        Assert.Null(opts.Bitrate);
        Assert.Null(opts.SampleRate);
        Assert.Null(opts.Channels);
    }
}
