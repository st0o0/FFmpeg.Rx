using FFmpeg.Rx.Filters;

namespace FFmpeg.Rx.Models;

public abstract record FilterSpec;

public sealed record SimpleFilterSpec(
    IReadOnlyList<IVideoFilter>? VideoFilters = null,
    IReadOnlyList<IAudioFilter>? AudioFilters = null) : FilterSpec;

public sealed record ComplexFilterSpec(FilterGraph.FilterGraph Graph) : FilterSpec;
