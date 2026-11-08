using FFmpeg.Rx.Codecs;

namespace FFmpeg.Rx.Models;

public abstract record CodecSpec;

public sealed record StreamCopy : CodecSpec;

public sealed record VideoEncode(VideoCodec Codec, VideoEncodingOptions Options) : CodecSpec;

public sealed record AudioEncode(AudioCodec Codec, AudioEncodingOptions Options) : CodecSpec;

public sealed record SubtitleEncode(SubtitleCodec Codec) : CodecSpec;

public sealed record VideoEncodingOptions(
    QualitySpec? Quality = null,
    Preset? Preset = null,
    VideoProfile? Profile = null,
    Tune? Tune = null,
    PixelFormat? PixelFormat = null,
    Framerate? Framerate = null);

public sealed record AudioEncodingOptions(
    Bitrate? Bitrate = null,
    int? SampleRate = null,
    int? Channels = null);
