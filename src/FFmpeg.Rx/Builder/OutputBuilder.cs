using FFmpeg.Rx.Codecs;
using FFmpeg.Rx.Filters;
using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.Builder;

public sealed class OutputBuilder
{
    private readonly OutputDestination _destination;
    private List<StreamConfig>? _streams;
    private ContainerFormat? _container;
    private FilterConfig? _filters;
    private MetadataConfig? _metadata;
    private TimeRange? _timeRange;
    private bool _overwrite = true;
    private bool _fastStart;
    private List<string>? _movFlags;
    private List<string>? _labelMappings;
    private List<string>? _customArgs;

    internal OutputBuilder(OutputDestination destination)
    {
        _destination = destination;
    }

    public OutputBuilder Stream(int inputIndex, int streamIndex, StreamType type, Action<StreamBuilder> configure)
    {
        var builder = new StreamBuilder(inputIndex, streamIndex, type);
        configure(builder);
        _streams ??= [];
        _streams.Add(builder.Build());
        return this;
    }

    public OutputBuilder Stream(int inputIndex, StreamType type, Action<StreamBuilder> configure)
    {
        var builder = new StreamBuilder(inputIndex, null, type);
        configure(builder);
        _streams ??= [];
        _streams.Add(builder.Build());
        return this;
    }

    public OutputBuilder VideoFilter(Action<VideoFilterBuilder> configure)
    {
        var builder = new VideoFilterBuilder();
        configure(builder);
        _filters = new SimpleFilterConfig(builder.Filters, (_filters as SimpleFilterConfig)?.AudioFilters);
        return this;
    }

    public OutputBuilder AudioFilter(Action<AudioFilterBuilder> configure)
    {
        var builder = new AudioFilterBuilder();
        configure(builder);
        _filters = new SimpleFilterConfig((_filters as SimpleFilterConfig)?.VideoFilters, builder.Filters);
        return this;
    }

    public OutputBuilder FilterComplex(FilterGraph.FilterGraph graph)
    {
        _filters = new ComplexFilterConfig(graph);
        return this;
    }

    public OutputBuilder Container(ContainerFormat format)
    {
        _container = format;
        return this;
    }

    public OutputBuilder TimeRange(Action<TimeRangeBuilder> configure)
    {
        var builder = new TimeRangeBuilder();
        configure(builder);
        _timeRange = builder.Build();
        return this;
    }

    public OutputBuilder Metadata(Action<MetadataBuilder> configure)
    {
        var builder = new MetadataBuilder();
        configure(builder);
        _metadata = builder.Build();
        return this;
    }

    public OutputBuilder FastStart()
    {
        _fastStart = true;
        return this;
    }

    public OutputBuilder MovFlags(params string[] flags)
    {
        _movFlags ??= [];
        _movFlags.AddRange(flags);
        return this;
    }

    public OutputBuilder NoOverwrite()
    {
        _overwrite = false;
        return this;
    }

    public OutputBuilder MapLabel(string label)
    {
        _labelMappings ??= [];
        _labelMappings.Add(label);
        return this;
    }

    public OutputBuilder CustomArg(string arg)
    {
        _customArgs ??= [];
        _customArgs.Add(arg);
        return this;
    }

    internal OutputConfig Build()
    {
        return new OutputConfig(
            _destination,
            _streams,
            _container,
            _filters,
            _metadata,
            _timeRange,
            _overwrite,
            _fastStart,
            _movFlags,
            _labelMappings,
            null,
            _customArgs);
    }
}
