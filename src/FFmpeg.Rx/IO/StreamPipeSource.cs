namespace FFmpeg.Rx.IO;

public sealed class StreamPipeSource(Stream source, string format) : IPipeSource
{
    public string StreamFormat { get; } = format;

    public async Task WriteAsync(Stream pipeStream, CancellationToken ct)
    {
        await source.CopyToAsync(pipeStream, ct).ConfigureAwait(false);
    }
}
