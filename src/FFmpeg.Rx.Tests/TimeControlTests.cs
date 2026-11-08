using FFmpeg.Rx.Codecs;
using FFmpeg.Rx.Internal;
using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.Tests;

public sealed class TimeControlTests
{
    private static string SerializeWithInput(InputOptionConfig opts)
    {
        var spec = new PipelineConfig(
            [new FileInputConfig("input.mp4") { Options = opts }],
            [new OutputConfig(new FileDestination("out.mp4"))]);
        return ArgumentSerializer.Serialize(spec);
    }

    private static string SerializeWithOutput(OutputConfig output)
    {
        var spec = new PipelineConfig(
            [new FileInputConfig("input.mp4")],
            [output]);
        return ArgumentSerializer.Serialize(spec);
    }

    [Fact]
    public void Input_seek_emits_before_input()
    {
        var args = SerializeWithInput(new InputOptionConfig(Seek: TimeSpan.FromSeconds(30)));
        var ssIdx = args.IndexOf("-ss ", StringComparison.Ordinal);
        var iIdx = args.IndexOf("-i ", StringComparison.Ordinal);
        Assert.True(ssIdx < iIdx, "-ss should appear before -i");
    }

    [Fact]
    public void Output_seek()
    {
        var args = SerializeWithOutput(new OutputConfig(new FileDestination("out.mp4"),
            TimeRange: new TimeRange(Seek: TimeSpan.FromSeconds(30))));
        Assert.Contains("-ss 00:00:30.000", args);
    }

    [Fact]
    public void Duration_limit()
    {
        var args = SerializeWithOutput(new OutputConfig(new FileDestination("out.mp4"),
            TimeRange: new TimeRange(Duration: TimeSpan.FromSeconds(10))));
        Assert.Contains("-t 10.000", args);
    }

    [Fact]
    public void Stop_time()
    {
        var args = SerializeWithOutput(new OutputConfig(new FileDestination("out.mp4"),
            TimeRange: new TimeRange(StopTime: TimeSpan.FromMinutes(1))));
        Assert.Contains("-to 00:01:00.000", args);
    }

    [Fact]
    public void Input_loop()
    {
        var args = SerializeWithInput(new InputOptionConfig(Loop: true));
        Assert.Contains("-loop 1", args);
    }

    [Fact]
    public void Stream_loop()
    {
        var args = SerializeWithInput(new InputOptionConfig(StreamLoop: 3));
        Assert.Contains("-stream_loop 3", args);
    }

    [Fact]
    public void Input_framerate()
    {
        var args = SerializeWithInput(new InputOptionConfig(Framerate: Framerate.Fps(30)));
        Assert.Contains("-framerate 30", args);
    }
}
