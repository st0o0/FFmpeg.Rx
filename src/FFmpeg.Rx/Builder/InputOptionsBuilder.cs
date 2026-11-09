using FFmpeg.Rx.Codecs;
using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.Builder;

public sealed class InputOptionsBuilder
{
    private TimeSpan? _seek;
    private HWAccel? _hwAccel;
    private string? _hwAccelDevice;
    private string? _hwAccelOutputFormat;
    private VideoCodec? _videoDecoder;
    private Framerate? _framerate;
    private bool _loop;
    private int? _streamLoop;
    private string? _userAgent;
    private string? _httpProxy;
    private string? _headers;
    private List<string>? _customArgs;

    public InputOptionsBuilder Seek(TimeSpan value)
    {
        _seek = value;
        return this;
    }

    public InputOptionsBuilder HWAccel(HWAccel value)
    {
        _hwAccel = value;
        return this;
    }

    public InputOptionsBuilder HWAccelDevice(string value)
    {
        _hwAccelDevice = value;
        return this;
    }

    public InputOptionsBuilder HWAccelOutputFormat(string value)
    {
        _hwAccelOutputFormat = value;
        return this;
    }

    public InputOptionsBuilder Decoder(VideoCodec value)
    {
        _videoDecoder = value;
        return this;
    }

    public InputOptionsBuilder Framerate(Framerate value)
    {
        _framerate = value;
        return this;
    }

    public InputOptionsBuilder Loop()
    {
        _loop = true;
        return this;
    }

    public InputOptionsBuilder StreamLoop(int count)
    {
        _streamLoop = count;
        return this;
    }

    public InputOptionsBuilder UserAgent(string value)
    {
        _userAgent = value;
        return this;
    }

    public InputOptionsBuilder HttpProxy(string value)
    {
        _httpProxy = value;
        return this;
    }

    public InputOptionsBuilder Headers(string value)
    {
        _headers = value;
        return this;
    }

    public InputOptionsBuilder CustomArg(string arg)
    {
        _customArgs ??= [];
        _customArgs.Add(arg);
        return this;
    }

    internal InputOptionConfig Build()
    {
        return new InputOptionConfig(
            _seek,
            _hwAccel,
            _hwAccelDevice,
            _hwAccelOutputFormat,
            _videoDecoder,
            _framerate,
            _loop,
            _streamLoop,
            _userAgent,
            _httpProxy,
            _headers,
            _customArgs);
    }
}
