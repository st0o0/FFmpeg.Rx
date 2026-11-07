using FFmpeg.Rx.Codecs;
using FFmpeg.Rx.Internal;
using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.Tests;

public sealed class MultiOutputTests
{
    [Fact]
    public void Multiple_outputs_in_single_command()
    {
        var pipeline = new FfmpegPipeline(
            [new FileInput("input.mp4")],
            [
                new FfmpegOutput("hd.mp4", [new StreamMapping(0, 0, StreamType.Video, VideoCodec.LibX264)], ["-crf 18"]),
                new FfmpegOutput("sd.mp4", [new StreamMapping(0, 0, StreamType.Video, VideoCodec.LibX264)], ["-crf 28"])
            ],
            []);

        var args = ArgumentBuilder.Build(pipeline);
        Assert.Contains("-crf 18", args);
        Assert.Contains("\"hd.mp4\"", args);
        Assert.Contains("-crf 28", args);
        Assert.Contains("\"sd.mp4\"", args);
    }

    [Fact]
    public void Tee_muxer_two_targets()
    {
        var pipeline = new FfmpegPipeline(
            [new FileInput("input.mp4")],
            [new FfmpegOutput("", [new StreamMapping(0, 0, StreamType.Video, VideoCodec.LibX264)], [])],
            [],
            TeeOutput: new TeeOutput([
                new TeeTarget("out1.mp4", Format: "mp4"),
                new TeeTarget("out2.mp4", Format: "mp4")
            ]));

        var args = ArgumentBuilder.Build(pipeline);
        Assert.Contains("-f tee", args);
        Assert.Contains("[f=mp4]out1.mp4|[f=mp4]out2.mp4", args);
    }

    [Fact]
    public void Tee_with_select_and_ignore()
    {
        var pipeline = new FfmpegPipeline(
            [new FileInput("input.mp4")],
            [new FfmpegOutput("", [], [])],
            [],
            TeeOutput: new TeeOutput([
                new TeeTarget("video.mp4", Format: "mp4", Select: "v"),
                new TeeTarget("audio.mp4", Format: "mp4", Select: "a", IgnoreFailure: true)
            ]));

        var args = ArgumentBuilder.Build(pipeline);
        Assert.Contains("[f=mp4:select=v]video.mp4", args);
        Assert.Contains("[f=mp4:select=a:onfail=ignore]audio.mp4", args);
    }

    [Fact]
    public void Null_output()
    {
        var session = Ffmpeg.Input("input.mp4")
            .OutputNull(o => o.AudioFilter(f => f.SilenceDetect(-30, 2)));

        Assert.NotNull(session);
    }
}
