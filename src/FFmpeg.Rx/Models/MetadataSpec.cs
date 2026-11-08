namespace FFmpeg.Rx.Models;

public sealed record MetadataSpec(
    IReadOnlyList<KeyValuePair<string, string>>? GlobalEntries = null,
    bool StripAll = false,
    int? CopyFromInput = null);
