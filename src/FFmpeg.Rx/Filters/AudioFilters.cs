using System.Globalization;

namespace FFmpeg.Rx.Filters;

public sealed record VolumeFilter(double Factor) : IAudioFilter
{
    public string Render()
    {
        return string.Create(CultureInfo.InvariantCulture, $"volume={Factor}");
    }
}

public sealed record VolumeExprFilter(string Expression) : IAudioFilter
{
    public string Render()
    {
        return $"volume={Expression}";
    }
}

public sealed record LoudNormFilter(double? I = null, double? Tp = null, double? Lra = null) : IAudioFilter
{
    public string Render()
    {
        if (I is null && Tp is null && Lra is null)
        {
            return "loudnorm";
        }

        var parts = new List<string>();
        if (I.HasValue)
        {
            parts.Add(string.Create(CultureInfo.InvariantCulture, $"I={I.Value}"));
        }

        if (Tp.HasValue)
        {
            parts.Add(string.Create(CultureInfo.InvariantCulture, $"TP={Tp.Value}"));
        }

        if (Lra.HasValue)
        {
            parts.Add(string.Create(CultureInfo.InvariantCulture, $"LRA={Lra.Value}"));
        }

        return "loudnorm=" + string.Join(":", parts);
    }
}

public sealed record AResampleFilter(int SampleRate) : IAudioFilter
{
    public string Render()
    {
        return $"aresample={SampleRate}";
    }
}

public sealed record SilenceDetectFilter(double NoiseDb, double Duration) : IAudioFilter
{
    public string Render()
    {
        return string.Create(CultureInfo.InvariantCulture,
            $"silencedetect=n={NoiseDb}dB:d={Duration}");
    }
}
