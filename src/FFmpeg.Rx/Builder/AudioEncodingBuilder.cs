using FFmpeg.Rx.Codecs;
using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.Builder;

public sealed class AudioEncodingBuilder
{
    private Bitrate? _bitrate;
    private int? _sampleRate;
    private int? _channels;

    public AudioEncodingBuilder Bitrate(Bitrate bitrate)
    {
        _bitrate = bitrate;
        return this;
    }

    public AudioEncodingBuilder SampleRate(int sampleRate)
    {
        _sampleRate = sampleRate;
        return this;
    }

    public AudioEncodingBuilder Channels(int channels)
    {
        _channels = channels;
        return this;
    }

    internal AudioEncodingOptions Build()
    {
        return new AudioEncodingOptions(_bitrate, _sampleRate, _channels);
    }
}
