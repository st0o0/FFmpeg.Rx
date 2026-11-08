using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.Builder;

public sealed class TeeBuilder
{
    private readonly List<TeeTarget> _targets = [];

    public TeeBuilder Target(string path, Action<TeeTargetOptions>? configure = null)
    {
        var options = new TeeTargetOptions();
        configure?.Invoke(options);
        _targets.Add(new TeeTarget(path, options.Format, options.Select,
            options.BitstreamFilter, options.MovFlags, options.IgnoreFailure));
        return this;
    }

    internal TeeOutput Build() => new([.. _targets]);
}

public sealed class TeeTargetOptions
{
    internal string? Format { get; private set; }
    internal string? Select { get; private set; }
    internal string? BitstreamFilter { get; private set; }
    internal string? MovFlags { get; private set; }
    internal bool IgnoreFailure { get; private set; }

    public TeeTargetOptions ForceFormat(string format) { Format = format; return this; }
    public TeeTargetOptions WithSelect(string select) { Select = select; return this; }
    public TeeTargetOptions WithBitstreamFilter(string bsf) { BitstreamFilter = bsf; return this; }
    public TeeTargetOptions WithMovFlags(string flags) { MovFlags = flags; return this; }
    public TeeTargetOptions OnFailIgnore() { IgnoreFailure = true; return this; }
}
