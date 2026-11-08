namespace FFmpeg.Rx.Codecs;

public abstract record QualitySpec;

public sealed record ConstantQuality(int Crf) : QualitySpec;

public sealed record ConstantBitrate(Bitrate Rate) : QualitySpec;

public static class Quality
{
    public static QualitySpec Crf(int value)
    {
        return new ConstantQuality(value);
    }

    public static QualitySpec Bitrate(Bitrate rate)
    {
        return new ConstantBitrate(rate);
    }
}
