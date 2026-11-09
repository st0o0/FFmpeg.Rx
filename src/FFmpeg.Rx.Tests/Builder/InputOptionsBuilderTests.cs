using FFmpeg.Rx.Builder;
using FFmpeg.Rx.Codecs;
using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.Tests.Builder;

public sealed class InputOptionsBuilderTests
{
    [Fact]
    public void Seek_sets_value()
    {
        var builder = new InputOptionsBuilder();
        builder.Seek(TimeSpan.FromSeconds(30));
        var opts = builder.Build();
        Assert.Equal(TimeSpan.FromSeconds(30), opts.Seek);
    }

    [Fact]
    public void HWAccel_sets_value()
    {
        var builder = new InputOptionsBuilder();
        builder.HWAccel(Codecs.HWAccel.Cuda);
        var opts = builder.Build();
        Assert.Equal(Codecs.HWAccel.Cuda, opts.HWAccel);
    }

    [Fact]
    public void HWAccelDevice_sets_value()
    {
        var builder = new InputOptionsBuilder();
        builder.HWAccelDevice("0");
        var opts = builder.Build();
        Assert.Equal("0", opts.HWAccelDevice);
    }

    [Fact]
    public void HWAccelOutputFormat_sets_value()
    {
        var builder = new InputOptionsBuilder();
        builder.HWAccelOutputFormat("cuda");
        var opts = builder.Build();
        Assert.Equal("cuda", opts.HWAccelOutputFormat);
    }

    [Fact]
    public void Decoder_sets_video_decoder()
    {
        var builder = new InputOptionsBuilder();
        builder.Decoder(VideoCodec.H264Cuvid);
        var opts = builder.Build();
        Assert.Equal(VideoCodec.H264Cuvid, opts.VideoDecoder);
    }

    [Fact]
    public void Framerate_sets_value()
    {
        var builder = new InputOptionsBuilder();
        builder.Framerate(Framerate.Fps(25));
        var opts = builder.Build();
        Assert.Equal(Framerate.Fps(25), opts.Framerate);
    }

    [Fact]
    public void Loop_sets_true()
    {
        var builder = new InputOptionsBuilder();
        builder.Loop();
        var opts = builder.Build();
        Assert.True(opts.Loop);
    }

    [Fact]
    public void StreamLoop_sets_count()
    {
        var builder = new InputOptionsBuilder();
        builder.StreamLoop(3);
        var opts = builder.Build();
        Assert.Equal(3, opts.StreamLoop);
    }

    [Fact]
    public void UserAgent_sets_value()
    {
        var builder = new InputOptionsBuilder();
        builder.UserAgent("Mozilla/5.0");
        var opts = builder.Build();
        Assert.Equal("Mozilla/5.0", opts.UserAgent);
    }

    [Fact]
    public void HttpProxy_sets_value()
    {
        var builder = new InputOptionsBuilder();
        builder.HttpProxy("http://proxy:8888");
        var opts = builder.Build();
        Assert.Equal("http://proxy:8888", opts.HttpProxy);
    }

    [Fact]
    public void Headers_sets_value()
    {
        var builder = new InputOptionsBuilder();
        builder.Headers("Referer: https://example.com");
        var opts = builder.Build();
        Assert.Equal("Referer: https://example.com", opts.Headers);
    }

    [Fact]
    public void CustomArg_accumulates()
    {
        var builder = new InputOptionsBuilder();
        builder.CustomArg("-reconnect 1").CustomArg("-reconnect_streamed 1");
        var opts = builder.Build();
        Assert.NotNull(opts.CustomArgs);
        Assert.Equal(2, opts.CustomArgs.Count);
        Assert.Equal("-reconnect 1", opts.CustomArgs[0]);
        Assert.Equal("-reconnect_streamed 1", opts.CustomArgs[1]);
    }

    [Fact]
    public void Fluent_chaining_all_http_options()
    {
        var builder = new InputOptionsBuilder();
        var opts = builder
            .UserAgent("Mozilla/5.0")
            .HttpProxy("http://proxy:8888")
            .Headers("Referer: https://example.com")
            .Build();

        Assert.Equal("Mozilla/5.0", opts.UserAgent);
        Assert.Equal("http://proxy:8888", opts.HttpProxy);
        Assert.Equal("Referer: https://example.com", opts.Headers);
    }

    [Fact]
    public void Default_build_returns_empty_config()
    {
        var opts = new InputOptionsBuilder().Build();
        Assert.Null(opts.Seek);
        Assert.Null(opts.HWAccel);
        Assert.Null(opts.UserAgent);
        Assert.Null(opts.HttpProxy);
        Assert.Null(opts.Headers);
        Assert.Null(opts.CustomArgs);
        Assert.False(opts.Loop);
    }
}
