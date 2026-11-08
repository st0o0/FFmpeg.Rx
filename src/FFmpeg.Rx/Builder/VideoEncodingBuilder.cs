using FFmpeg.Rx.Codecs;
using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.Builder;

public sealed class VideoEncodingBuilder
{
    private QualitySpec? _quality;
    private Preset? _preset;
    private VideoProfile? _profile;
    private Tune? _tune;
    private PixelFormat? _pixelFormat;
    private Framerate? _framerate;

    public VideoEncodingBuilder Quality(QualitySpec quality)
    {
        _quality = quality;
        return this;
    }

    public VideoEncodingBuilder Preset(Preset preset)
    {
        _preset = preset;
        return this;
    }

    public VideoEncodingBuilder Profile(VideoProfile profile)
    {
        _profile = profile;
        return this;
    }

    public VideoEncodingBuilder Tune(Tune tune)
    {
        _tune = tune;
        return this;
    }

    public VideoEncodingBuilder PixelFormat(PixelFormat format)
    {
        _pixelFormat = format;
        return this;
    }

    public VideoEncodingBuilder Framerate(Framerate framerate)
    {
        _framerate = framerate;
        return this;
    }

    internal VideoEncodingOptions Build()
    {
        return new VideoEncodingOptions(_quality, _preset, _profile, _tune, _pixelFormat, _framerate);
    }
}
