namespace FFmpeg.Rx.Models;

public sealed record MetadataConfig(
    IReadOnlyList<KeyValuePair<string, string>>? GlobalEntries = null,
    bool StripAll = false,
    int? CopyFromInput = null);
