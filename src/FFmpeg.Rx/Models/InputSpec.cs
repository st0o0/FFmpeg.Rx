using FFmpeg.Rx.Codecs;
using FFmpeg.Rx.IO;

namespace FFmpeg.Rx.Models;

public abstract record InputSpec
{
    public InputOptionSpec? Options { get; init; }
}

public sealed record FileInputSpec(string Path) : InputSpec;

public sealed record UrlInputSpec(Uri Url) : InputSpec;

public sealed record PipeInputSpec(IPipeSource Source, string PipeName) : InputSpec;

public sealed record InputOptionSpec(
    TimeSpan? Seek = null,
    HWAccel? HWAccel = null,
    string? HWAccelDevice = null,
    string? HWAccelOutputFormat = null,
    VideoCodec? VideoDecoder = null,
    Framerate? Framerate = null,
    bool Loop = false,
    int? StreamLoop = null,
    IReadOnlyList<string>? CustomArgs = null);
