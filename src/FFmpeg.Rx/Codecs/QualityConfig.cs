namespace FFmpeg.Rx.Codecs;

public abstract record QualityConfig;

public sealed record ConstantQuality(int Crf) : QualityConfig;

public sealed record ConstantBitrate(Bitrate Rate) : QualityConfig;

public static class Quality
{
    public static QualityConfig Crf(int value) => new ConstantQuality(value);

    public static QualityConfig Bitrate(Bitrate rate) => new ConstantBitrate(rate);
}
