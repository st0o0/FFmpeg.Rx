using FFmpeg.Rx.Builder;
using FFmpeg.Rx.Codecs;
using FFmpeg.Rx.Internal;
using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.Tests;

public sealed class FluentApiTests
{
    [Fact]
    public void Input_string_creates_FileInputConfig()
    {
        var builder = new PipelineBuilder(null);
        builder.Input("video.mp4");
        var session = builder.Output("out.mp4", o => o.Stream(0, 0, StreamType.Video, s => s.Copy()));
        Assert.IsAssignableFrom<IFfmpegSession>(session);
    }

    [Fact]
    public void Input_Uri_creates_UrlInputConfig()
    {
        var session = Ffmpeg.Input(new Uri("https://example.com/v.mp4"))
            .Output("out.mp4", o => o.Stream(0, 0, StreamType.Video, s => s.Copy()));
        Assert.IsAssignableFrom<IFfmpegSession>(session);
    }

    [Fact]
    public void Input_Stream_creates_PipeInputConfig()
    {
        using var stream = new MemoryStream();
        var session = Ffmpeg.Input(stream, "rawvideo")
            .Output("out.mp4", o => o.Stream(0, 0, StreamType.Video, s => s.Copy()));
        Assert.IsAssignableFrom<IFfmpegSession>(session);
    }

    [Fact]
    public void At_sets_custom_binary_path()
    {
        var session = Ffmpeg.At("/custom/ffmpeg")
            .Input("video.mp4")
            .Output("out.mp4", o => o.Stream(0, 0, StreamType.Video, s => s.Copy()));
        Assert.IsAssignableFrom<IFfmpegSession>(session);
    }

    [Fact]
    public void Output_returns_IFfmpegSession()
    {
        var session = Ffmpeg.Input("video.mp4")
            .Output("out.mp4", o => o.Stream(0, 0, StreamType.Video, s => s.Copy()));
        Assert.IsAssignableFrom<IFfmpegSession>(session);
    }

    [Fact]
    public void OutputPipe_returns_IFfmpegSession()
    {
        using var stream = new MemoryStream();
        var session = Ffmpeg.Input("video.mp4")
            .OutputPipe(stream, o => o.Container(ContainerFormat.Mp4));
        Assert.IsAssignableFrom<IFfmpegSession>(session);
    }

    [Fact]
    public void OutputNull_returns_IFfmpegSession()
    {
        var session = Ffmpeg.Input("video.mp4")
            .OutputNull(o => o.Stream(0, 0, StreamType.Video, s => s.Copy()));
        Assert.IsAssignableFrom<IFfmpegSession>(session);
    }

    [Fact]
    public void OutputTee_returns_IFfmpegSession()
    {
        var session = Ffmpeg.Input("video.mp4")
            .OutputTee(
                o => o.Stream(0, 0, StreamType.Video, s => s.Copy()),
                t => t.Target("out1.mp4").Target("out2.mp4"));
        Assert.IsAssignableFrom<IFfmpegSession>(session);
    }

    [Fact]
    public void OutputTee_with_options_returns_IFfmpegSession()
    {
        var session = Ffmpeg.Input("video.mp4")
            .OutputTee(
                o => o.Stream(0, 0, StreamType.Video, s => s.Copy()),
                t => t
                    .Target("out1.mp4", opts => opts
                        .ForceFormat("mp4")
                        .WithSelect("v")
                        .WithMovFlags("faststart"))
                    .Target("out2.mp4", opts => opts
                        .ForceFormat("mp4")
                        .WithBitstreamFilter("aac_adtstoasc")
                        .OnFailIgnore()));
        Assert.IsAssignableFrom<IFfmpegSession>(session);
    }

    [Fact]
    public void Outputs_multi_build_returns_IFfmpegSession()
    {
        var session = Ffmpeg.Input("video.mp4")
            .Outputs()
            .Output("hd.mp4", o => o.Stream(0, 0, StreamType.Video, s => s.Copy()))
            .Output("sd.mp4", o => o.Stream(0, 0, StreamType.Video, s => s.Copy()))
            .Build();
        Assert.IsAssignableFrom<IFfmpegSession>(session);
    }

    [Fact]
    public void WithProgressReporting_adds_global_option()
    {
        var spec = new PipelineConfig(
            [new FileInputConfig("video.mp4")],
            [new OutputConfig(new FileDestination("out.mp4"))],
            GlobalOptions: ["-progress pipe:1"]);
        var args = ArgumentSerializer.Serialize(spec);
        Assert.Contains("-progress pipe:1", args);
    }

    [Fact]
    public void Multiple_inputs()
    {
        var session = Ffmpeg.Input("video.mp4")
            .Input("subs.srt")
            .Output("out.mkv", o => o
                .Stream(0, 0, StreamType.Video, s => s.Copy())
                .Stream(1, 0, StreamType.Subtitle, s => s.Encode(SubtitleCodec.Srt)));
        Assert.IsAssignableFrom<IFfmpegSession>(session);
    }
}
