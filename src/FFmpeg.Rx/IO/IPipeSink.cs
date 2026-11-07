namespace FFmpeg.Rx.IO;

public interface IPipeSink
{
    Task ReadAsync(Stream pipeStream, CancellationToken ct);
}
