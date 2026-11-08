using FFmpeg.Rx.Codecs;

namespace FFmpeg.Rx.Models;

public sealed record OutputConfig(
    OutputDestination Destination,
    IReadOnlyList<StreamConfig>? Streams = null,
    ContainerFormat? Container = null,
    FilterConfig? Filters = null,
    MetadataConfig? Metadata = null,
    TimeRange? TimeRange = null,
    bool Overwrite = true,
    bool FastStart = false,
    IReadOnlyList<string>? MovFlags = null,
    IReadOnlyList<string>? LabelMappings = null,
    TeeOutput? Tee = null,
    IReadOnlyList<string>? CustomArgs = null);
