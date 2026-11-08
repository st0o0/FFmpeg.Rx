using FFmpeg.Rx.Codecs;

namespace FFmpeg.Rx.Models;

public sealed record OutputSpec(
    OutputDestination Destination,
    IReadOnlyList<StreamSpec>? Streams = null,
    ContainerFormat? Container = null,
    FilterSpec? Filters = null,
    MetadataSpec? Metadata = null,
    TimeRange? TimeRange = null,
    bool Overwrite = true,
    bool FastStart = false,
    IReadOnlyList<string>? MovFlags = null,
    IReadOnlyList<string>? LabelMappings = null,
    TeeOutput? Tee = null,
    IReadOnlyList<string>? CustomArgs = null);
