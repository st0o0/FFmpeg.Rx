using FFmpeg.Rx.Builder;
using FFmpeg.Rx.Models;

namespace FFmpeg.Rx;

public static class Ffmpeg
{
    public static PipelineBuilder Input(Uri url)
    {
        return new PipelineBuilder(null).AddInput(new UrlInputSpec(url));
    }

    public static PipelineBuilder Input(string filePath)
    {
        return new PipelineBuilder(null).AddInput(new FileInputSpec(filePath));
    }

    public static PipelineBuilder Input(Stream source, string format)
    {
        return new PipelineBuilder(null).AddInput(new PipeInputSpec(source, format));
    }

    public static PipelineBuilder At(string binaryPath)
    {
        return new PipelineBuilder(binaryPath);
    }
}
