using FFmpeg.Rx.Internal;
using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.Tests;

public sealed class FilterGraphTests
{
    [Fact]
    public void Single_chain()
    {
        var graph = FilterGraph.FilterGraph.Create()
            .Input(0, StreamType.Video, "base")
            .Chain("base", f => f.Scale(1920, 1080), "scaled")
            .Build();

        Assert.Equal("[0:v]scale=1920:1080[scaled]", graph.Serialize());
    }

    [Fact]
    public void Multi_input_overlay()
    {
        var graph = FilterGraph.FilterGraph.Create()
            .Input(0, StreamType.Video, "base")
            .Input(1, StreamType.Video, "logo")
            .Chain("base", f => f.Scale(1920, 1080), "scaled")
            .Merge("scaled", "logo", f => f.Overlay(10, 10), "out")
            .Build();

        Assert.Equal("[0:v]scale=1920:1080[scaled];[scaled][logo]overlay=10:10[out]", graph.Serialize());
    }

    [Fact]
    public void Split()
    {
        var graph = FilterGraph.FilterGraph.Create()
            .Input(0, StreamType.Video, "base")
            .Split("base", "copy1", "copy2")
            .Build();

        Assert.Equal("[base]split[copy1][copy2]", graph.Serialize());
    }

    [Fact]
    public void ASplit()
    {
        var graph = FilterGraph.FilterGraph.Create()
            .Input(0, StreamType.Audio, "audio")
            .ASplit("audio", "a1", "a2")
            .Build();

        Assert.Equal("[audio]asplit[a1][a2]", graph.Serialize());
    }

    [Fact]
    public void Concat()
    {
        var graph = FilterGraph.FilterGraph.Create()
            .Input(0, StreamType.Video, "v1")
            .Input(1, StreamType.Video, "v2")
            .Merge("v1", "v2", f => f.Concat(2, true, true), "out")
            .Build();

        Assert.Contains("concat=n=2:v=1:a=1", graph.Serialize());
    }

    [Fact]
    public void AudioMix()
    {
        var graph = FilterGraph.FilterGraph.Create()
            .Input(0, StreamType.Audio, "a1")
            .Input(1, StreamType.Audio, "a2")
            .Merge("a1", "a2", f => f.AudioMix(2), "out")
            .Build();

        Assert.Contains("amix=inputs=2", graph.Serialize());
    }

    [Fact]
    public void BurnSubtitles()
    {
        var graph = FilterGraph.FilterGraph.Create()
            .Input(0, StreamType.Video, "base")
            .Chain("base", f => f.BurnSubtitles("subs.srt"), "out")
            .Build();

        Assert.Equal("[0:v]subtitles=subs.srt[out]", graph.Serialize());
    }

    [Fact]
    public void FilterComplex_in_argument_builder()
    {
        var graph = FilterGraph.FilterGraph.Create()
            .Input(0, StreamType.Video, "base")
            .Chain("base", f => f.Scale(1920, 1080), "out")
            .Build();

        var spec = new PipelineSpec(
            [new FileInputSpec("in.mp4")],
            [new OutputSpec(
                new FileDestination("out.mp4"),
                Filters: new ComplexFilterSpec(graph),
                LabelMappings: ["out"])]);

        var args = ArgumentSerializer.Serialize(spec);
        Assert.Contains("-filter_complex \"[0:v]scale=1920:1080[out]\"", args);
        Assert.Contains("-map \"[out]\"", args);
    }

    [Fact]
    public void Overlay_with_expression()
    {
        var graph = FilterGraph.FilterGraph.Create()
            .Input(0, StreamType.Video, "base")
            .Input(1, StreamType.Video, "logo")
            .Merge("base", "logo", f => f.Overlay("(W-w)/2", "(H-h)/2"), "out")
            .Build();

        Assert.Contains("overlay=(W-w)/2:(H-h)/2", graph.Serialize());
    }
}
