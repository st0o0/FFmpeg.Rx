using FFmpeg.Rx.Codecs;
using FFmpeg.Rx.Internal;
using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.Tests;

public sealed class EcosystemExtrasTests
{
    private static string BuildWith(Action<Builder.OutputOptions> configure)
    {
        var options = new Builder.OutputOptions();
        configure(options);
        var pipeline = new FfmpegPipeline(
            [new FileInput("input.mp4")],
            new FfmpegOutput("out.mp4", [.. options.Mappings], [.. options.Arguments], options.SuppressOverwrite),
            []);
        return ArgumentBuilder.Build(pipeline);
    }

    [Fact]
    public void Negative_map()
    {
        var args = BuildWith(o => o.NegativeMap(0, StreamType.Audio));
        Assert.Contains("-map -0:a", args);
    }

    [Fact]
    public void Codec_tag()
    {
        var args = BuildWith(o => o.CodecTag("hvc1", StreamType.Video));
        Assert.Contains("-tag:v hvc1", args);
    }

    [Fact]
    public void MovFlags_single()
    {
        var args = BuildWith(o => o.MovFlags("faststart"));
        Assert.Contains("-movflags faststart", args);
    }

    [Fact]
    public void MovFlags_multiple()
    {
        var args = BuildWith(o => o.MovFlags("frag_keyframe", "empty_moov"));
        Assert.Contains("-movflags frag_keyframe+empty_moov", args);
    }

    [Fact]
    public void MapAll()
    {
        var args = BuildWith(o => o.MapAll(0));
        Assert.Contains("-map 0", args);
    }

    [Fact]
    public void CopyStreams()
    {
        var args = BuildWith(o => o.CopyStreams());
        Assert.Contains("-c copy", args);
    }

    [Fact]
    public void Tune()
    {
        var args = BuildWith(o => o.Tune(EncoderTune.Film));
        Assert.Contains("-tune film", args);
    }

    [Fact]
    public void Image_sequence_input()
    {
        var options = new Builder.InputOptions();
        options.Framerate(30);
        var pipeline = new FfmpegPipeline(
            [new FileInput("frames/img_%04d.png", options.Arguments)],
            new FfmpegOutput("out.mp4", [], []),
            []);
        var args = ArgumentBuilder.Build(pipeline);
        Assert.Contains("-framerate 30", args);
        Assert.Contains("-i \"frames/img_%04d.png\"", args);
    }

    [Fact]
    public void Stream_recording_pattern()
    {
        var pipeline = new FfmpegPipeline(
            [new UrlInput(new Uri("rtmp://live.example.com/stream"))],
            new FfmpegOutput("recording.ts",
                [new StreamMapping(0, 0, StreamType.Video, VideoCodec.Copy),
                 new StreamMapping(0, 0, StreamType.Audio, AudioCodec.Copy)],
                ["-t 60"]),
            []);
        var args = ArgumentBuilder.Build(pipeline);
        Assert.Contains("-i rtmp://live.example.com/stream", args);
        Assert.Contains("-c:v copy", args);
        Assert.Contains("-c:a copy", args);
        Assert.Contains("-t 60", args);
    }

    [Fact]
    public void Subtitle_mux_pattern()
    {
        var pipeline = new FfmpegPipeline(
            [new FileInput("video.mp4"), new FileInput("subs.srt")],
            new FfmpegOutput("out.mkv",
                [new StreamMapping(0, 0, StreamType.Video, VideoCodec.Copy),
                 new StreamMapping(0, 0, StreamType.Audio, AudioCodec.Copy),
                 new StreamMapping(1, 0, StreamType.Subtitle, SubtitleCodec.Srt)],
                []),
            []);
        var args = ArgumentBuilder.Build(pipeline);
        Assert.Contains("-map 1:s:0", args);
        Assert.Contains("-c:s srt", args);
    }
}
