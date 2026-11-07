using FFmpeg.Rx.Codecs;
using FFmpeg.Rx.Internal;
using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.Tests;

public sealed class ArgumentBuilderTests
{
    [Fact]
    public void Url_input_with_proxy_and_user_agent()
    {
        var pipeline = new FfmpegPipeline(
            [new UrlInput(new Uri("https://example.com/video.mp4"), UserAgent: "Mozilla/5.0", Proxy: "http://proxy:8888")],
            new FfmpegOutput("out.mkv",
                [new StreamMapping(0, 0, StreamType.Video, VideoCodec.Copy)],
                []),
            []);

        var args = ArgumentBuilder.Build(pipeline);
        Assert.Contains("-user_agent \"Mozilla/5.0\"", args);
        Assert.Contains("-http_proxy http://proxy:8888", args);
        Assert.Contains("-i https://example.com/video.mp4", args);
    }

    [Fact]
    public void File_input_is_quoted()
    {
        var pipeline = new FfmpegPipeline(
            [new FileInput("/path/to/subs.srt")],
            new FfmpegOutput("out.mkv", [], []),
            []);

        var args = ArgumentBuilder.Build(pipeline);
        Assert.Contains("-i \"/path/to/subs.srt\"", args);
    }

    [Fact]
    public void Stream_mappings_and_codecs()
    {
        var pipeline = new FfmpegPipeline(
            [new UrlInput(new Uri("https://example.com/v.mp4"))],
            new FfmpegOutput("out.mkv",
                [new StreamMapping(0, 0, StreamType.Video, VideoCodec.Copy),
                 new StreamMapping(0, 0, StreamType.Audio, VideoCodec.Copy)],
                []),
            []);

        var args = ArgumentBuilder.Build(pipeline);
        Assert.Contains("-map 0:v:0", args);
        Assert.Contains("-map 0:a:0", args);
        Assert.Contains("-c:v copy", args);
        Assert.Contains("-c:a copy", args);
    }

    [Fact]
    public void Multi_input_with_subtitle()
    {
        var pipeline = new FfmpegPipeline(
            [new UrlInput(new Uri("https://example.com/v.mp4")),
             new FileInput("/subs.srt")],
            new FfmpegOutput("out.mkv",
                [new StreamMapping(0, 0, StreamType.Video, VideoCodec.Copy),
                 new StreamMapping(0, 0, StreamType.Audio, VideoCodec.Copy),
                 new StreamMapping(1, 0, StreamType.Subtitle, "srt")],
                []),
            []);

        var args = ArgumentBuilder.Build(pipeline);
        Assert.Contains("-map 1:s:0", args);
        Assert.Contains("-c:s srt", args);
    }

    [Fact]
    public void Progress_global_option()
    {
        var pipeline = new FfmpegPipeline(
            [new UrlInput(new Uri("https://example.com/v.mp4"))],
            new FfmpegOutput("out.mkv", [], []),
            ["-progress pipe:1"]);

        var args = ArgumentBuilder.Build(pipeline);
        Assert.StartsWith("-progress pipe:1", args);
    }

    [Fact]
    public void Custom_output_arguments()
    {
        var pipeline = new FfmpegPipeline(
            [new UrlInput(new Uri("https://example.com/v.mp4"))],
            new FfmpegOutput("out.mkv", [],
                ["-bsf:a aac_adtstoasc", "-metadata:s:s:0 language=deu"]),
            []);

        var args = ArgumentBuilder.Build(pipeline);
        Assert.Contains("-bsf:a aac_adtstoasc", args);
        Assert.Contains("-metadata:s:s:0 language=deu", args);
    }

    [Fact]
    public void Output_path_is_last_with_overwrite()
    {
        var pipeline = new FfmpegPipeline(
            [new UrlInput(new Uri("https://example.com/v.mp4"))],
            new FfmpegOutput("/output/file.mkv", [], []),
            []);

        var args = ArgumentBuilder.Build(pipeline);
        Assert.EndsWith("-y \"/output/file.mkv\"", args);
    }
}
