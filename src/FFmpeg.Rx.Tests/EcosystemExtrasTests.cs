using FFmpeg.Rx.Codecs;
using FFmpeg.Rx.Internal;
using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.Tests;

public sealed class EcosystemExtrasTests
{
    [Fact]
    public void Negative_map_via_custom_arg()
    {
        var spec = new PipelineSpec(
            [new FileInputSpec("input.mp4")],
            [new OutputSpec(new FileDestination("out.mp4"),
                CustomArgs: ["-map -0:a"])]);

        var args = ArgumentSerializer.Serialize(spec);
        Assert.Contains("-map -0:a", args);
    }

    [Fact]
    public void Codec_tag_via_custom_arg()
    {
        var spec = new PipelineSpec(
            [new FileInputSpec("input.mp4")],
            [new OutputSpec(new FileDestination("out.mp4"),
                CustomArgs: ["-tag:v hvc1"])]);

        var args = ArgumentSerializer.Serialize(spec);
        Assert.Contains("-tag:v hvc1", args);
    }

    [Fact]
    public void MovFlags_single()
    {
        var spec = new PipelineSpec(
            [new FileInputSpec("input.mp4")],
            [new OutputSpec(new FileDestination("out.mp4"), MovFlags: ["faststart"])]);

        var args = ArgumentSerializer.Serialize(spec);
        Assert.Contains("-movflags faststart", args);
    }

    [Fact]
    public void MovFlags_multiple()
    {
        var spec = new PipelineSpec(
            [new FileInputSpec("input.mp4")],
            [new OutputSpec(new FileDestination("out.mp4"), MovFlags: ["frag_keyframe", "empty_moov"])]);

        var args = ArgumentSerializer.Serialize(spec);
        Assert.Contains("-movflags frag_keyframe+empty_moov", args);
    }

    [Fact]
    public void FastStart_emits_movflags()
    {
        var spec = new PipelineSpec(
            [new FileInputSpec("input.mp4")],
            [new OutputSpec(new FileDestination("out.mp4"), FastStart: true)]);

        var args = ArgumentSerializer.Serialize(spec);
        Assert.Contains("-movflags +faststart", args);
    }

    [Fact]
    public void CopyStreams_via_stream_copy()
    {
        var spec = new PipelineSpec(
            [new FileInputSpec("input.mp4")],
            [new OutputSpec(new FileDestination("out.mp4"),
                Streams: [
                    new StreamSpec(0, 0, StreamType.Video, new StreamCopy()),
                    new StreamSpec(0, 0, StreamType.Audio, new StreamCopy())])]);

        var args = ArgumentSerializer.Serialize(spec);
        Assert.Contains("-c:v copy", args);
        Assert.Contains("-c:a copy", args);
    }

    [Fact]
    public void Image_sequence_input()
    {
        var spec = new PipelineSpec(
            [new FileInputSpec("frames/img_%04d.png") { Options = new InputOptionSpec(Framerate: Framerate.Fps(30)) }],
            [new OutputSpec(new FileDestination("out.mp4"))]);

        var args = ArgumentSerializer.Serialize(spec);
        Assert.Contains("-framerate 30", args);
        Assert.Contains("-i \"frames/img_%04d.png\"", args);
    }

    [Fact]
    public void Stream_recording_pattern()
    {
        var spec = new PipelineSpec(
            [new UrlInputSpec(new Uri("rtmp://live.example.com/stream"))],
            [new OutputSpec(new FileDestination("recording.ts"),
                Streams: [
                    new StreamSpec(0, 0, StreamType.Video, new StreamCopy()),
                    new StreamSpec(0, 0, StreamType.Audio, new StreamCopy())],
                TimeRange: new TimeRange(Duration: TimeSpan.FromSeconds(60)))]);

        var args = ArgumentSerializer.Serialize(spec);
        Assert.Contains("-i rtmp://live.example.com/stream", args);
        Assert.Contains("-c:v copy", args);
        Assert.Contains("-c:a copy", args);
        Assert.Contains("-t 60.000", args);
    }

    [Fact]
    public void Subtitle_mux_pattern()
    {
        var spec = new PipelineSpec(
            [new FileInputSpec("video.mp4"), new FileInputSpec("subs.srt")],
            [new OutputSpec(new FileDestination("out.mkv"),
                Streams: [
                    new StreamSpec(0, 0, StreamType.Video, new StreamCopy()),
                    new StreamSpec(0, 0, StreamType.Audio, new StreamCopy()),
                    new StreamSpec(1, 0, StreamType.Subtitle, new SubtitleEncode(SubtitleCodec.Srt))])]);

        var args = ArgumentSerializer.Serialize(spec);
        Assert.Contains("-map 1:s:0", args);
        Assert.Contains("-c:s srt", args);
    }
}
