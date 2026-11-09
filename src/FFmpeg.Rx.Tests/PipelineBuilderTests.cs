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
    public void PipelineConfig_single_input_single_output()
    {
        var spec = new PipelineConfig(
            [new FileInputConfig("video.mp4")],
            [new OutputConfig(new FileDestination("out.mp4"),
                Streams: [new StreamConfig(0, 0, StreamType.Video, new StreamCopy())])]);

        Assert.Single(spec.Inputs);
        Assert.Single(spec.Outputs);
        Assert.IsType<FileInputConfig>(spec.Inputs[0]);
        Assert.IsType<FileDestination>(spec.Output.Destination);
        Assert.IsType<StreamCopy>(spec.Output.Streams![0].Codec);
    }

    [Fact]
    public void PipelineConfig_multiple_inputs()
    {
        var spec = new PipelineConfig(
            [new UrlInputConfig(new Uri("https://example.com/v.mp4")),
             new FileInputConfig("/subs.srt")],
            [new OutputConfig(new FileDestination("out.mkv"),
                Streams: [
                    new StreamConfig(0, 0, StreamType.Video, new StreamCopy()),
                    new StreamConfig(1, 0, StreamType.Subtitle, new SubtitleEncode(SubtitleCodec.Srt))])]);

        Assert.Equal(2, spec.Inputs.Count);
        Assert.IsType<UrlInputConfig>(spec.Inputs[0]);
        Assert.IsType<FileInputConfig>(spec.Inputs[1]);
    }

    [Fact]
    public void PipelineConfig_with_binary_path()
    {
        var spec = new PipelineConfig(
            [new FileInputConfig("input.mp4")],
            [new OutputConfig(new FileDestination("out.mp4"))],
            BinaryPath: "/usr/local/bin/ffmpeg");

        Assert.Equal("/usr/local/bin/ffmpeg", spec.BinaryPath);
    }

    [Fact]
    public void PipelineConfig_serializes_roundtrip()
    {
        var spec = new PipelineConfig(
            [new FileInputConfig("video.mp4")],
            [new OutputConfig(new FileDestination("out.mkv"),
                Streams: [new StreamConfig(0, 0, StreamType.Video, new StreamCopy())],
                CustomArgs: ["-bsf:a aac_adtstoasc"])]);

        var args = ArgumentSerializer.Serialize(spec);
        Assert.Contains("-map 0:v:0", args);
        Assert.Contains("-c:v copy", args);
        Assert.Contains("-bsf:a aac_adtstoasc", args);
        Assert.EndsWith("-y \"out.mkv\"", args.TrimEnd());
    }

    [Fact]
    public void InputOptionConfig_http_properties()
    {
        var opts = new InputOptionConfig(
            UserAgent: "Mozilla/5.0",
            HttpProxy: "http://proxy:8888",
            Headers: "Referer: https://example.com");

        Assert.Equal("Mozilla/5.0", opts.UserAgent);
        Assert.Equal("http://proxy:8888", opts.HttpProxy);
        Assert.Equal("Referer: https://example.com", opts.Headers);
    }

    [Fact]
    public void StreamConfig_with_bitstream_filter()
    {
        var stream = new StreamConfig(0, 0, StreamType.Audio, new StreamCopy(),
            BitstreamFilter: "aac_adtstoasc");

        Assert.Equal("aac_adtstoasc", stream.BitstreamFilter);
    }

    [Fact]
    public void StreamConfig_bitstream_filter_null_by_default()
    {
        var stream = new StreamConfig(0, 0, StreamType.Video, new StreamCopy());

        Assert.Null(stream.BitstreamFilter);
    }

    [Fact]
    public void Builder_url_input_with_options()
    {
        var session = Ffmpeg
            .Input(new Uri("https://example.com/video.mp4"), o => o.UserAgent("Mozilla/5.0"))
            .Output("out.mkv", o => o.Stream(0, 0, StreamType.Video, s => s.Copy()));

        Assert.IsAssignableFrom<IFfmpegSession>(session);
    }

    [Fact]
    public void Builder_file_input_with_options()
    {
        var session = Ffmpeg
            .Input("video.mp4", o => o.Seek(TimeSpan.FromSeconds(10)))
            .Output("out.mkv", o => o.Stream(0, 0, StreamType.Video, s => s.Copy()));

        Assert.IsAssignableFrom<IFfmpegSession>(session);
    }

    [Fact]
    public void AddInput_is_public()
    {
        var builder = new FFmpeg.Rx.Builder.PipelineBuilder(null);
        var input = new UrlInputConfig(new Uri("https://example.com/v.mp4"))
        {
            Options = new InputOptionConfig(UserAgent: "Bot")
        };
        var result = builder.AddInput(input);
        Assert.Same(builder, result);
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
