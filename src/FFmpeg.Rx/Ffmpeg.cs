using FFmpeg.Rx.Builder;
using FFmpeg.Rx.Models;

namespace FFmpeg.Rx;

public static class Ffmpeg
{
    public static PipelineBuilder Input(Uri url, Action<InputOptions>? configure = null)
    {
        var options = new InputOptions();
        configure?.Invoke(options);
        return new PipelineBuilder(null).AddInput(
            new UrlInput(url, options.UserAgentValue, options.ProxyValue, options.Arguments));
    }

    public static PipelineBuilder Input(string filePath, Action<InputOptions>? configure = null)
    {
        var options = new InputOptions();
        configure?.Invoke(options);
        return new PipelineBuilder(null).AddInput(new FileInput(filePath, options.Arguments));
    }

    public static PipelineBuilder At(string binaryPath)
    {
        return new PipelineBuilder(binaryPath);
    }
}
