using FFmpeg.Rx.Codecs;

namespace FFmpeg.Rx.Models;

public abstract record InputConfig
{
    public InputOptionConfig? Options { get; init; }
}

public sealed record FileInputConfig(string Path) : InputConfig;

public sealed record UrlInputConfig(Uri Url) : InputConfig;

public sealed record PipeInputConfig(Stream Source, string Format) : InputConfig;

public sealed record InputOptionConfig(
    TimeSpan? Seek = null,
    HWAccel? HWAccel = null,
    string? HWAccelDevice = null,
    string? HWAccelOutputFormat = null,
    VideoCodec? VideoDecoder = null,
    Framerate? Framerate = null,
    bool Loop = false,
    int? StreamLoop = null,
    string? UserAgent = null,
    string? HttpProxy = null,
    string? Headers = null,
    IReadOnlyList<string>? CustomArgs = null);
