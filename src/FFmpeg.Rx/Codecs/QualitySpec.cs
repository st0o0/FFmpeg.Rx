namespace FFmpeg.Rx.Codecs;

public abstract record QualitySpec;

public sealed record ConstantQuality(int Crf) : QualitySpec;

public sealed record ConstantBitrate(Bitrate Rate) : QualitySpec;

public static class Quality
{
    public static QualitySpec Crf(int value) => new ConstantQuality(value);

    public static QualitySpec Bitrate(Bitrate rate) => new ConstantBitrate(rate);
}
