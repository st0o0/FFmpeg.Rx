using System.Globalization;

namespace FFmpeg.Rx.Codecs;

public readonly record struct Bitrate
{
    public long BitsPerSecond { get; }

    private Bitrate(long bitsPerSecond) => BitsPerSecond = bitsPerSecond;

    public static Bitrate Kbps(int kilobits) => new(kilobits * 1000L);
    public static Bitrate Mbps(int megabits) => new(megabits * 1_000_000L);
    public static Bitrate Mbps(double megabits) => new((long)(megabits * 1_000_000));

    internal string ToFfmpegString()
    {
        if (BitsPerSecond % 1_000_000 == 0)
        {
            return string.Create(CultureInfo.InvariantCulture, $"{BitsPerSecond / 1_000_000}M");
        }

        if (BitsPerSecond % 1000 == 0)
        {
            return string.Create(CultureInfo.InvariantCulture, $"{BitsPerSecond / 1000}k");
        }
        return BitsPerSecond.ToString(CultureInfo.InvariantCulture);
    }

    public override string ToString() => ToFfmpegString();
}
