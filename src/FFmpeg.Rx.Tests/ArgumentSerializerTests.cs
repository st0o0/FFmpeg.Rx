using FFmpeg.Rx.Codecs;
using FFmpeg.Rx.Internal;
using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.Tests;

public sealed class ArgumentSerializerTests
{
    [Fact]
    public void Url_input()
    {
        var spec = new PipelineSpec(
            [new UrlInputSpec(new Uri("https://example.com/video.mp4"))],
            [new OutputSpec(new FileDestination("out.mkv"))]);

        var args = ArgumentSerializer.Serialize(spec);
        Assert.Contains("-i https://example.com/video.mp4", args);
    }

    [Fact]
    public void File_input_is_quoted()
    {
        var spec = new PipelineSpec(
            [new FileInputSpec("/path/to/subs.srt")],
            [new OutputSpec(new FileDestination("out.mkv"))]);

        var args = ArgumentSerializer.Serialize(spec);
        Assert.Contains("-i \"/path/to/subs.srt\"", args);
    }

    [Fact]
    public void Stream_mappings_and_codecs()
    {
        var spec = new PipelineSpec(
            [new UrlInputSpec(new Uri("https://example.com/v.mp4"))],
            [new OutputSpec(new FileDestination("out.mkv"),
                Streams: [
                    new StreamSpec(0, 0, StreamType.Video, new StreamCopy()),
                    new StreamSpec(0, 0, StreamType.Audio, new StreamCopy())])]);

        var args = ArgumentSerializer.Serialize(spec);
        Assert.Contains("-map 0:v:0", args);
        Assert.Contains("-map 0:a:0", args);
        Assert.Contains("-c:v copy", args);
        Assert.Contains("-c:a copy", args);
    }

    [Fact]
    public void Multi_input_with_subtitle()
    {
        var spec = new PipelineSpec(
            [new UrlInputSpec(new Uri("https://example.com/v.mp4")),
             new FileInputSpec("/subs.srt")],
            [new OutputSpec(new FileDestination("out.mkv"),
                Streams: [
                    new StreamSpec(0, 0, StreamType.Video, new StreamCopy()),
                    new StreamSpec(0, 0, StreamType.Audio, new StreamCopy()),
                    new StreamSpec(1, 0, StreamType.Subtitle, new SubtitleEncode(SubtitleCodec.Srt))])]);

        var args = ArgumentSerializer.Serialize(spec);
        Assert.Contains("-map 1:s:0", args);
        Assert.Contains("-c:s srt", args);
    }

    [Fact]
    public void Progress_global_option()
    {
        var spec = new PipelineSpec(
            [new UrlInputSpec(new Uri("https://example.com/v.mp4"))],
            [new OutputSpec(new FileDestination("out.mkv"))],
            GlobalOptions: ["-progress pipe:1"]);

        var args = ArgumentSerializer.Serialize(spec);
        Assert.StartsWith("-progress pipe:1", args);
    }

    [Fact]
    public void Custom_output_arguments()
    {
        var spec = new PipelineSpec(
            [new UrlInputSpec(new Uri("https://example.com/v.mp4"))],
            [new OutputSpec(new FileDestination("out.mkv"),
                CustomArgs: ["-bsf:a aac_adtstoasc", "-metadata:s:s:0 language=deu"])]);

        var args = ArgumentSerializer.Serialize(spec);
        Assert.Contains("-bsf:a aac_adtstoasc", args);
        Assert.Contains("-metadata:s:s:0 language=deu", args);
    }

    [Fact]
    public void Output_path_is_last_with_overwrite()
    {
        var spec = new PipelineSpec(
            [new UrlInputSpec(new Uri("https://example.com/v.mp4"))],
            [new OutputSpec(new FileDestination("/output/file.mkv"))]);

        var args = ArgumentSerializer.Serialize(spec);
        Assert.EndsWith("-y \"/output/file.mkv\"", args.TrimEnd());
    }

    [Fact]
    public void No_overwrite_emits_n_flag()
    {
        var spec = new PipelineSpec(
            [new FileInputSpec("input.mp4")],
            [new OutputSpec(new FileDestination("out.mp4"), Overwrite: false)]);

        var args = ArgumentSerializer.Serialize(spec);
        Assert.Contains("-n \"out.mp4\"", args);
        Assert.DoesNotContain("-y", args);
    }

    [Fact]
    public void Video_encode_with_crf_and_preset()
    {
        var spec = new PipelineSpec(
            [new FileInputSpec("input.mp4")],
            [new OutputSpec(new FileDestination("out.mp4"),
                Streams: [new StreamSpec(0, 0, StreamType.Video,
                    new VideoEncode(VideoCodec.H264,
                        new VideoEncodingOptions(
                            Quality: Quality.Crf(23),
                            Preset: Preset.Slow)))])]);

        var args = ArgumentSerializer.Serialize(spec);
        Assert.Contains("-c:v libx264", args);
        Assert.Contains("-crf 23", args);
        Assert.Contains("-preset slow", args);
    }

    [Fact]
    public void Audio_encode_with_bitrate()
    {
        var spec = new PipelineSpec(
            [new FileInputSpec("input.mp4")],
            [new OutputSpec(new FileDestination("out.mp4"),
                Streams: [new StreamSpec(0, 0, StreamType.Audio,
                    new AudioEncode(AudioCodec.Aac,
                        new AudioEncodingOptions(Bitrate: Bitrate.Kbps(192))))])]);

        var args = ArgumentSerializer.Serialize(spec);
        Assert.Contains("-c:a aac", args);
        Assert.Contains("-b:a 192k", args);
    }

    [Fact]
    public void Container_format_emits_f_flag()
    {
        var spec = new PipelineSpec(
            [new FileInputSpec("input.mp4")],
            [new OutputSpec(new FileDestination("out.ts"),
                Container: ContainerFormat.Ts)]);

        var args = ArgumentSerializer.Serialize(spec);
        Assert.Contains("-f mpegts", args);
    }

    [Fact]
    public void Null_destination()
    {
        var spec = new PipelineSpec(
            [new FileInputSpec("input.mp4")],
            [new OutputSpec(NullDestination.Instance)]);

        var args = ArgumentSerializer.Serialize(spec);
        Assert.Contains("/dev/null", args);
    }

    [Fact]
    public void Progress_rewritten_to_stderr_with_pipe_output()
    {
        var spec = new PipelineSpec(
            [new FileInputSpec("input.mp4")],
            [new OutputSpec(new PipeDestination(Stream.Null))],
            GlobalOptions: ["-progress pipe:1"]);

        var args = ArgumentSerializer.Serialize(spec);
        Assert.Contains("-progress pipe:2", args);
        Assert.DoesNotContain("-progress pipe:1", args);
    }

    [Fact]
    public void Progress_stays_stdout_without_pipe_output()
    {
        var spec = new PipelineSpec(
            [new FileInputSpec("input.mp4")],
            [new OutputSpec(new FileDestination("out.mp4"))],
            GlobalOptions: ["-progress pipe:1"]);

        var args = ArgumentSerializer.Serialize(spec);
        Assert.Contains("-progress pipe:1", args);
    }
}
