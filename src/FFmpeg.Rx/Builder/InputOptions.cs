using System.Globalization;

namespace FFmpeg.Rx.Builder;

public sealed class InputOptions
{
    private readonly List<string> _args = [];

    internal string? UserAgentValue { get; private set; }
    internal string? ProxyValue { get; private set; }
    internal string? HWAccelValue { get; private set; }
    internal string? HWAccelDeviceValue { get; private set; }
    internal string? HWAccelOutputFormatValue { get; private set; }
    internal string? VideoDecoderValue { get; private set; }
    internal string[]? Arguments => _args.Count > 0 ? [.. _args] : null;

    public InputOptions UserAgent(string userAgent)
    {
        UserAgentValue = userAgent;
        return this;
    }

    public InputOptions Proxy(string proxy)
    {
        ProxyValue = proxy;
        return this;
    }

    public InputOptions Arg(string argument)
    {
        _args.Add(argument);
        return this;
    }

    public InputOptions Seek(TimeSpan position)
    {
        _args.Add($"-ss {FormatTime(position)}");
        return this;
    }

    public InputOptions Loop()
    {
        _args.Add("-loop 1");
        return this;
    }

    public InputOptions StreamLoop(int count)
    {
        _args.Add($"-stream_loop {count}");
        return this;
    }

    public InputOptions Framerate(double fps)
    {
        _args.Add(string.Create(CultureInfo.InvariantCulture, $"-framerate {fps}"));
        return this;
    }

    public InputOptions HWAccel(string accel)
    {
        _args.Add($"-hwaccel {accel}");
        return this;
    }

    public InputOptions HWAccelDevice(string device)
    {
        _args.Add($"-hwaccel_device {device}");
        return this;
    }

    public InputOptions HWAccelOutputFormat(string format)
    {
        _args.Add($"-hwaccel_output_format {format}");
        return this;
    }

    public InputOptions VideoDecoder(string decoder)
    {
        _args.Add($"-c:v {decoder}");
        return this;
    }

    private static string FormatTime(TimeSpan ts)
    {
        return ts.ToString(@"hh\:mm\:ss\.fff", CultureInfo.InvariantCulture);
    }
}
