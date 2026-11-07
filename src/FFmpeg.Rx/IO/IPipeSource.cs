namespace FFmpeg.Rx.IO;

public interface IPipeSource
{
    string StreamFormat { get; }
    Task WriteAsync(Stream pipeStream, CancellationToken ct);
}
