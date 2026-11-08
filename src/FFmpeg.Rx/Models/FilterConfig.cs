using FFmpeg.Rx.Filters;

namespace FFmpeg.Rx.Models;

public abstract record FilterConfig;

public sealed record SimpleFilterConfig(
    IReadOnlyList<IVideoFilter>? VideoFilters = null,
    IReadOnlyList<IAudioFilter>? AudioFilters = null) : FilterConfig;

public sealed record ComplexFilterConfig(FilterGraph.FilterGraph Graph) : FilterConfig;
