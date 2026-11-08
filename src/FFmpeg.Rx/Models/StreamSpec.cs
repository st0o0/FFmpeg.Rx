namespace FFmpeg.Rx.Models;

public sealed record StreamSpec(
    int InputIndex,
    int? StreamIndex,
    StreamType Type,
    CodecSpec Codec,
    IReadOnlyList<KeyValuePair<string, string>>? Metadata = null,
    string? Disposition = null);
