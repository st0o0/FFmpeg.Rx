using FFmpeg.Rx.Codecs;
using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.Builder;

public sealed class StreamBuilder
{
    private readonly int _inputIndex;
    private readonly int? _streamIndex;
    private readonly StreamType _type;
    private CodecSpec? _codec;
    private List<KeyValuePair<string, string>>? _metadata;
    private string? _disposition;

    internal StreamBuilder(int inputIndex, int? streamIndex, StreamType type)
    {
        _inputIndex = inputIndex;
        _streamIndex = streamIndex;
        _type = type;
    }

    public StreamBuilder Copy()
    {
        _codec = new StreamCopy();
        return this;
    }

    public StreamBuilder Encode(VideoCodec codec, Action<VideoEncodingBuilder>? configure = null)
    {
        var builder = new VideoEncodingBuilder();
        configure?.Invoke(builder);
        _codec = new VideoEncode(codec, builder.Build());
        return this;
    }

    public StreamBuilder Encode(AudioCodec codec, Action<AudioEncodingBuilder>? configure = null)
    {
        var builder = new AudioEncodingBuilder();
        configure?.Invoke(builder);
        _codec = new AudioEncode(codec, builder.Build());
        return this;
    }

    public StreamBuilder Encode(SubtitleCodec codec)
    {
        _codec = new SubtitleEncode(codec);
        return this;
    }

    public StreamBuilder Metadata(string key, string value)
    {
        _metadata ??= [];
        _metadata.Add(new(key, value));
        return this;
    }

    public StreamBuilder Disposition(string flags)
    {
        _disposition = flags;
        return this;
    }

    internal StreamSpec Build()
    {
        return new StreamSpec(
            _inputIndex,
            _streamIndex,
            _type,
            _codec ?? new StreamCopy(),
            _metadata,
            _disposition);
    }
}
