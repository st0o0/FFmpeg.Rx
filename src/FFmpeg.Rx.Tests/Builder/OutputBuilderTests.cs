using FFmpeg.Rx.Builder;
using FFmpeg.Rx.Codecs;
using FFmpeg.Rx.FilterGraph;
using FFmpeg.Rx.Filters;
using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.Tests.Builder;

public sealed class OutputBuilderTests
{
    private static OutputSpec Build(Action<OutputBuilder> configure)
    {
        var builder = new OutputBuilder(new FileDestination("out.mp4"));
        configure(builder);
        return builder.Build();
    }

    [Fact]
    public void Stream_adds_stream_spec()
    {
        var spec = Build(o => o.Stream(0, 0, StreamType.Video, s => s.Copy()));
        Assert.NotNull(spec.Streams);
        var stream = Assert.Single(spec.Streams);
        Assert.IsType<StreamCopy>(stream.Codec);
    }

    [Fact]
    public void Multiple_streams()
    {
        var spec = Build(o => o
            .Stream(0, 0, StreamType.Video, s => s.Copy())
            .Stream(0, 0, StreamType.Audio, s => s.Copy()));
        Assert.NotNull(spec.Streams);
        Assert.Equal(2, spec.Streams.Count);
    }

    [Fact]
    public void Stream_without_stream_index()
    {
        var spec = Build(o => o.Stream(0, StreamType.Video, s => s.Copy()));
        Assert.NotNull(spec.Streams);
        var stream = Assert.Single(spec.Streams);
        Assert.Null(stream.StreamIndex);
    }

    [Fact]
    public void VideoFilter_sets_simple_filter()
    {
        var spec = Build(o => o.VideoFilter(f => f.Scale(1920, 1080)));
        var simple = Assert.IsType<SimpleFilterSpec>(spec.Filters);
        Assert.NotNull(simple.VideoFilters);
        Assert.Single(simple.VideoFilters);
    }

    [Fact]
    public void AudioFilter_sets_simple_filter()
    {
        var spec = Build(o => o.AudioFilter(f => f.Volume(1.5)));
        var simple = Assert.IsType<SimpleFilterSpec>(spec.Filters);
        Assert.NotNull(simple.AudioFilters);
        Assert.Single(simple.AudioFilters);
    }

    [Fact]
    public void FilterComplex_sets_complex_filter()
    {
        var graph = new FilterGraphBuilder()
            .Input(0, StreamType.Video, "v_in")
            .Chain("v_in", f => f.Scale(1920, 1080), "v_out")
            .Build();
        var spec = Build(o => o.FilterComplex(graph));
        Assert.IsType<ComplexFilterSpec>(spec.Filters);
    }

    [Fact]
    public void Container_sets_format()
    {
        var spec = Build(o => o.Container(ContainerFormat.Mkv));
        Assert.Equal(ContainerFormat.Mkv, spec.Container);
    }

    [Fact]
    public void TimeRange_sets_range()
    {
        var spec = Build(o => o.TimeRange(t => t.Seek(TimeSpan.FromSeconds(10))));
        Assert.NotNull(spec.TimeRange);
        Assert.Equal(TimeSpan.FromSeconds(10), spec.TimeRange.Seek);
    }

    [Fact]
    public void Metadata_sets_spec()
    {
        var spec = Build(o => o.Metadata(m => m.Global("title", "Test")));
        Assert.NotNull(spec.Metadata);
        Assert.NotNull(spec.Metadata.GlobalEntries);
        Assert.Single(spec.Metadata.GlobalEntries);
    }

    [Fact]
    public void FastStart_sets_flag()
    {
        var spec = Build(o => o.FastStart());
        Assert.True(spec.FastStart);
    }

    [Fact]
    public void MovFlags_adds_flags()
    {
        var spec = Build(o => o.MovFlags("frag_keyframe", "empty_moov"));
        Assert.NotNull(spec.MovFlags);
        Assert.Equal(2, spec.MovFlags.Count);
    }

    [Fact]
    public void NoOverwrite_sets_flag()
    {
        var spec = Build(o => o.NoOverwrite());
        Assert.False(spec.Overwrite);
    }

    [Fact]
    public void Default_overwrite_is_true()
    {
        var spec = Build(_ => { });
        Assert.True(spec.Overwrite);
    }

    [Fact]
    public void MapLabel_adds_label()
    {
        var spec = Build(o => o.MapLabel("v_out"));
        Assert.NotNull(spec.LabelMappings);
        Assert.Single(spec.LabelMappings);
        Assert.Equal("v_out", spec.LabelMappings[0]);
    }

    [Fact]
    public void CustomArg_adds_arg()
    {
        var spec = Build(o => o.CustomArg("-bsf:a aac_adtstoasc"));
        Assert.NotNull(spec.CustomArgs);
        Assert.Single(spec.CustomArgs);
        Assert.Equal("-bsf:a aac_adtstoasc", spec.CustomArgs[0]);
    }

    [Fact]
    public void Destination_is_file()
    {
        var spec = Build(_ => { });
        var dest = Assert.IsType<FileDestination>(spec.Destination);
        Assert.Equal("out.mp4", dest.Path);
    }
}
