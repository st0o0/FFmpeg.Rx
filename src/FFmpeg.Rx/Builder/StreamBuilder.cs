using FFmpeg.Rx.Codecs;
using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.Builder;

public sealed class StreamBuilder
{
    private readonly int _inputIndex;
    private readonly int? _streamIndex;
    private readonly StreamType _type;
    private CodecConfig? _codec;
    private List<KeyValuePair<string, string>>? _metadata;
    private string? _disposition;
    private string? _bitstreamFilter;

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

    public StreamBuilder BitstreamFilter(string filter)
    {
        _bitstreamFilter = filter;
        return this;
    }

    internal StreamConfig Build()
    {
        return new StreamConfig(
            _inputIndex,
            _streamIndex,
            _type,
            _codec ?? new StreamCopy(),
            _metadata,
            _disposition,
            _bitstreamFilter);
    }
}
