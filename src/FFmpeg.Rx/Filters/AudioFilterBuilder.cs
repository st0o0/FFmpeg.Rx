namespace FFmpeg.Rx.Filters;

public sealed class AudioFilterBuilder
{
    private readonly List<IAudioFilter> _filters = [];

    internal IReadOnlyList<IAudioFilter> Filters => _filters;

    public AudioFilterBuilder Volume(double factor)
    {
        _filters.Add(new VolumeFilter(factor));
        return this;
    }

    public AudioFilterBuilder Volume(string expression)
    {
        _filters.Add(new VolumeExprFilter(expression));
        return this;
    }

    public AudioFilterBuilder LoudNorm(double? i = null, double? tp = null, double? lra = null)
    {
        _filters.Add(new LoudNormFilter(i, tp, lra));
        return this;
    }

    public AudioFilterBuilder AResample(int sampleRate)
    {
        _filters.Add(new AResampleFilter(sampleRate));
        return this;
    }

    public AudioFilterBuilder SilenceDetect(double noiseDb, double duration)
    {
        _filters.Add(new SilenceDetectFilter(noiseDb, duration));
        return this;
    }

    public AudioFilterBuilder Custom(string name, string? args = null)
    {
        _filters.Add(new CustomFilter(name, args));
        return this;
    }
}
