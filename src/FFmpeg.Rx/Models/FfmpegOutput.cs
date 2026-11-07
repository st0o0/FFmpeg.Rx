using FFmpeg.Rx.Filters;

namespace FFmpeg.Rx.Models;

public sealed record FfmpegOutput(
    string Path,
    StreamMapping[] Streams,
    string[] CustomArguments,
    bool NoOverwrite = false,
    IReadOnlyList<IVideoFilter>? VideoFilters = null,
    IReadOnlyList<IAudioFilter>? AudioFilters = null,
    string? FilterComplexString = null,
    string[]? LabelMappings = null);

public sealed record StreamMapping(
    int InputIndex,
    int StreamIndex,
    StreamType Type,
    string Codec);
