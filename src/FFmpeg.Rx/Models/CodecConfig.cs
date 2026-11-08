using FFmpeg.Rx.Codecs;

namespace FFmpeg.Rx.Models;

public abstract record CodecConfig;

public sealed record StreamCopy : CodecConfig;

public sealed record VideoEncode(VideoCodec Codec, VideoEncodingOptions Options) : CodecConfig;

public sealed record AudioEncode(AudioCodec Codec, AudioEncodingOptions Options) : CodecConfig;

public sealed record SubtitleEncode(SubtitleCodec Codec) : CodecConfig;

public sealed record VideoEncodingOptions(
    QualityConfig? Quality = null,
    Preset? Preset = null,
    VideoProfile? Profile = null,
    Tune? Tune = null,
    PixelFormat? PixelFormat = null,
    Framerate? Framerate = null);

public sealed record AudioEncodingOptions(
    Bitrate? Bitrate = null,
    int? SampleRate = null,
    int? Channels = null);
