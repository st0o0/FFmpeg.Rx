using FFmpeg.Rx.Internal;
using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.Tests;

public sealed class TimeControlTests
{
    private static string BuildWithInput(Action<Builder.InputOptions> configure)
    {
        var options = new Builder.InputOptions();
        configure(options);
        var pipeline = new FfmpegPipeline(
            [new FileInput("input.mp4", options.Arguments)],
            new FfmpegOutput("out.mp4", [], []),
            []);
        return ArgumentBuilder.Build(pipeline);
    }

    private static string BuildWithOutput(Action<Builder.OutputOptions> configure)
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
    public void Input_seek_emits_before_input()
    {
        var args = BuildWithInput(o => o.Seek(TimeSpan.FromSeconds(30)));
        var ssIdx = args.IndexOf("-ss ", StringComparison.Ordinal);
        var iIdx = args.IndexOf("-i ", StringComparison.Ordinal);
        Assert.True(ssIdx < iIdx, "-ss should appear before -i");
    }

    [Fact]
    public void Output_seek()
    {
        var args = BuildWithOutput(o => o.Seek(TimeSpan.FromSeconds(30)));
        Assert.Contains("-ss 00:00:30.000", args);
    }

    [Fact]
    public void Duration_limit()
    {
        var args = BuildWithOutput(o => o.Duration(TimeSpan.FromSeconds(10)));
        Assert.Contains("-t 10.000", args);
    }

    [Fact]
    public void Stop_time()
    {
        var args = BuildWithOutput(o => o.StopTime(TimeSpan.FromMinutes(1)));
        Assert.Contains("-to 00:01:00.000", args);
    }

    [Fact]
    public void Input_loop()
    {
        var args = BuildWithInput(o => o.Loop());
        Assert.Contains("-loop 1", args);
    }

    [Fact]
    public void Stream_loop()
    {
        var args = BuildWithInput(o => o.StreamLoop(3));
        Assert.Contains("-stream_loop 3", args);
    }

    [Fact]
    public void Video_speed_double()
    {
        var args = BuildWithOutput(o => o.VideoSpeed(2.0));
        Assert.Contains("-filter:v \"setpts=0.5*PTS\"", args);
    }

    [Fact]
    public void Audio_tempo_normal()
    {
        var args = BuildWithOutput(o => o.AudioTempo(2.0));
        Assert.Contains("-filter:a \"atempo=2\"", args);
    }

    [Fact]
    public void Audio_tempo_extreme_chains()
    {
        var args = BuildWithOutput(o => o.AudioTempo(4.0));
        Assert.Contains("atempo=2.0,atempo=2", args);
    }

    [Fact]
    public void Input_framerate()
    {
        var args = BuildWithInput(o => o.Framerate(30));
        Assert.Contains("-framerate 30", args);
    }
}
