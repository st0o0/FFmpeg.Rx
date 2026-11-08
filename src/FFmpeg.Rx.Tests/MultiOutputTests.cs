using FFmpeg.Rx.Codecs;
using FFmpeg.Rx.Internal;
using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.Tests;

public sealed class MultiOutputTests
{
    [Fact]
    public void Multiple_outputs_in_single_command()
    {
        var spec = new PipelineSpec(
            [new FileInputSpec("input.mp4")],
            [
                new OutputSpec(new FileDestination("hd.mp4"),
                    Streams: [new StreamSpec(0, 0, StreamType.Video,
                        new VideoEncode(VideoCodec.H264, new VideoEncodingOptions(Quality: Quality.Crf(18))))]),
                new OutputSpec(new FileDestination("sd.mp4"),
                    Streams: [new StreamSpec(0, 0, StreamType.Video,
                        new VideoEncode(VideoCodec.H264, new VideoEncodingOptions(Quality: Quality.Crf(28))))])
            ]);

        var args = ArgumentSerializer.Serialize(spec);
        Assert.Contains("-crf 18", args);
        Assert.Contains("\"hd.mp4\"", args);
        Assert.Contains("-crf 28", args);
        Assert.Contains("\"sd.mp4\"", args);
    }

    [Fact]
    public void Tee_muxer_two_targets()
    {
        var spec = new PipelineSpec(
            [new FileInputSpec("input.mp4")],
            [new OutputSpec(
                new FileDestination(""),
                Streams: [new StreamSpec(0, 0, StreamType.Video,
                    new VideoEncode(VideoCodec.H264, new VideoEncodingOptions()))],
                Tee: new TeeOutput([
                    new TeeTarget("out1.mp4", Format: "mp4"),
                    new TeeTarget("out2.mp4", Format: "mp4")]))]);

        var args = ArgumentSerializer.Serialize(spec);
        Assert.Contains("-f tee", args);
        Assert.Contains("[f=mp4]out1.mp4|[f=mp4]out2.mp4", args);
    }

    [Fact]
    public void Tee_with_select_and_ignore()
    {
        var spec = new PipelineSpec(
            [new FileInputSpec("input.mp4")],
            [new OutputSpec(
                new FileDestination(""),
                Tee: new TeeOutput([
                    new TeeTarget("video.mp4", Format: "mp4", Select: "v"),
                    new TeeTarget("audio.mp4", Format: "mp4", Select: "a", IgnoreFailure: true)]))]);

        var args = ArgumentSerializer.Serialize(spec);
        Assert.Contains("[f=mp4:select=v]video.mp4", args);
        Assert.Contains("[f=mp4:select=a:onfail=ignore]audio.mp4", args);
    }

    [Fact]
    public void Null_output_serializes()
    {
        var spec = new PipelineSpec(
            [new FileInputSpec("input.mp4")],
            [new OutputSpec(NullDestination.Instance, Container: ContainerFormat.Null)]);

        var args = ArgumentSerializer.Serialize(spec);
        Assert.Contains("-f null", args);
        Assert.Contains("/dev/null", args);
    }
}
