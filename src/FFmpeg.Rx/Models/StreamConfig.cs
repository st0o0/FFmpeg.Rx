namespace FFmpeg.Rx.Models;

public sealed record StreamConfig(
    int InputIndex,
    int? StreamIndex,
    StreamType Type,
    CodecConfig Codec,
    IReadOnlyList<KeyValuePair<string, string>>? Metadata = null,
    string? Disposition = null,
    string? BitstreamFilter = null);
