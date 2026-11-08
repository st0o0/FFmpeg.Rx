using FFmpeg.Rx.Codecs;
using FFmpeg.Rx.Internal;
using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.Tests;

public sealed class PipelineBuilderTests
{
    [Fact]
    public void Basic_pipeline_builds_session_via_builder()
    {
        var session = Ffmpeg
            .Input(new Uri("https://example.com/video.mp4"))
            .Output("out.mkv", o => o
                .Stream(0, 0, StreamType.Video, s => s.Copy())
                .Stream(0, 0, StreamType.Audio, s => s.Copy()));

        Assert.IsAssignableFrom<IFfmpegSession>(session);
    }

    [Fact]
    public void PipelineSpec_single_input_single_output()
    {
        var spec = new PipelineSpec(
            [new FileInputSpec("video.mp4")],
            [new OutputSpec(new FileDestination("out.mp4"),
                Streams: [new StreamSpec(0, 0, StreamType.Video, new StreamCopy())])]);

        Assert.Single(spec.Inputs);
        Assert.Single(spec.Outputs);
        Assert.IsType<FileInputSpec>(spec.Inputs[0]);
        Assert.IsType<FileDestination>(spec.Output.Destination);
        Assert.IsType<StreamCopy>(spec.Output.Streams![0].Codec);
    }

    [Fact]
    public void PipelineSpec_multiple_inputs()
    {
        var spec = new PipelineSpec(
            [new UrlInputSpec(new Uri("https://example.com/v.mp4")),
             new FileInputSpec("/subs.srt")],
            [new OutputSpec(new FileDestination("out.mkv"),
                Streams: [
                    new StreamSpec(0, 0, StreamType.Video, new StreamCopy()),
                    new StreamSpec(1, 0, StreamType.Subtitle, new SubtitleEncode(SubtitleCodec.Srt))])]);

        Assert.Equal(2, spec.Inputs.Count);
        Assert.IsType<UrlInputSpec>(spec.Inputs[0]);
        Assert.IsType<FileInputSpec>(spec.Inputs[1]);
    }

    [Fact]
    public void PipelineSpec_with_binary_path()
    {
        var spec = new PipelineSpec(
            [new FileInputSpec("input.mp4")],
            [new OutputSpec(new FileDestination("out.mp4"))],
            BinaryPath: "/usr/local/bin/ffmpeg");

        Assert.Equal("/usr/local/bin/ffmpeg", spec.BinaryPath);
    }

    [Fact]
    public void PipelineSpec_serializes_roundtrip()
    {
        var spec = new PipelineSpec(
            [new FileInputSpec("video.mp4")],
            [new OutputSpec(new FileDestination("out.mkv"),
                Streams: [new StreamSpec(0, 0, StreamType.Video, new StreamCopy())],
                CustomArgs: ["-bsf:a aac_adtstoasc"])]);

        var args = ArgumentSerializer.Serialize(spec);
        Assert.Contains("-map 0:v:0", args);
        Assert.Contains("-c:v copy", args);
        Assert.Contains("-bsf:a aac_adtstoasc", args);
        Assert.EndsWith("-y \"out.mkv\"", args.TrimEnd());
    }

    [Fact]
    public void VideoEncode_record_carries_all_options()
    {
        var encode = new VideoEncode(VideoCodec.H264,
            new VideoEncodingOptions(
                Quality: Quality.Crf(23),
                Preset: Preset.Slow,
                Profile: VideoProfile.High,
                Tune: Tune.Film,
                PixelFormat: PixelFormat.Yuv420p,
                Framerate: Framerate.Fps(30)));

        Assert.Equal("libx264", encode.Codec.Name);
        Assert.IsType<ConstantQuality>(encode.Options.Quality);
        Assert.Equal(23, ((ConstantQuality)encode.Options.Quality!).Crf);
        Assert.Equal("slow", encode.Options.Preset!.Value.Name);
        Assert.Equal("high", encode.Options.Profile!.Value.Name);
        Assert.Equal("film", encode.Options.Tune!.Value.Name);
        Assert.Equal("yuv420p", encode.Options.PixelFormat!.Value.Name);
        Assert.Equal(30, encode.Options.Framerate!.Value.Numerator);
    }
}
