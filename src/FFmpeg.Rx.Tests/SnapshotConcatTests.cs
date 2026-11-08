using FFmpeg.Rx.Internal;
using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.Tests;

public sealed class SnapshotConcatTests
{
    [Fact]
    public void Snapshot_serializes_seek_and_frames()
    {
        var spec = new PipelineConfig(
            [new FileInputConfig("video.mp4") { Options = new InputOptionConfig(Seek: TimeSpan.FromSeconds(5)) }],
            [new OutputConfig(
                new FileDestination("thumb.jpg"),
                CustomArgs: ["-frames:v 1", "-update 1"])]);

        var args = ArgumentSerializer.Serialize(spec);
        Assert.Contains("-ss 00:00:05.000", args);
        Assert.Contains("-frames:v 1", args);
        Assert.Contains("-update 1", args);
        Assert.Contains("\"thumb.jpg\"", args);
    }

    [Fact]
    public void Concat_serializes_concat_demuxer()
    {
        var spec = new PipelineConfig(
            [new FileInputConfig("list.txt") { Options = new InputOptionConfig(CustomArgs: ["-f concat", "-safe 0"]) }],
            [new OutputConfig(
                new FileDestination("output.mp4"),
                CustomArgs: ["-c copy"])]);

        var args = ArgumentSerializer.Serialize(spec);
        Assert.Contains("-f concat", args);
        Assert.Contains("-safe 0", args);
        Assert.Contains("-c copy", args);
        Assert.Contains("\"output.mp4\"", args);
    }

    [Fact]
    public void Probe_shortcut_exists()
    {
        var method = typeof(Ffmpeg).GetMethod("Probe", [typeof(string), typeof(CancellationToken)]);
        Assert.NotNull(method);
    }
}
