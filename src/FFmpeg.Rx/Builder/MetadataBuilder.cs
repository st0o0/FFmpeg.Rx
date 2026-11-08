using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.Builder;

public sealed class MetadataBuilder
{
    private List<KeyValuePair<string, string>>? _globalEntries;
    private bool _stripAll;
    private int? _copyFromInput;

    public MetadataBuilder Global(string key, string value)
    {
        _globalEntries ??= [];
        _globalEntries.Add(new(key, value));
        return this;
    }

    public MetadataBuilder StripAll()
    {
        _stripAll = true;
        return this;
    }

    public MetadataBuilder CopyFrom(int inputIndex)
    {
        _copyFromInput = inputIndex;
        return this;
    }

    internal MetadataConfig Build() => new(_globalEntries, _stripAll, _copyFromInput);
}
