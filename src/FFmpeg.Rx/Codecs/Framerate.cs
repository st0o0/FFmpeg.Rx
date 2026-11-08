using System.Globalization;

namespace FFmpeg.Rx.Codecs;

public readonly record struct Framerate
{
    public int Numerator { get; }
    public int Denominator { get; }

    private Framerate(int numerator, int denominator)
    {
        Numerator = numerator;
        Denominator = denominator;
    }

    public static Framerate Fps(int fps) => new(fps, 1);
    public static Framerate Fps(double fps) => new((int)(fps * 1000), 1000);
    public static Framerate Rational(int numerator, int denominator) => new(numerator, denominator);

    public static Framerate Ntsc24 => new(24000, 1001);
    public static Framerate Ntsc30 => new(30000, 1001);
    public static Framerate Ntsc60 => new(60000, 1001);

    public double ToDouble() => (double)Numerator / Denominator;

    internal string ToFfmpegString()
    {
        if (Denominator == 1)
        {
            return Numerator.ToString(CultureInfo.InvariantCulture);
        }
        return string.Create(CultureInfo.InvariantCulture, $"{Numerator}/{Denominator}");
    }

    public override string ToString() => ToFfmpegString();
}
