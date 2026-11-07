namespace FFmpeg.Rx.IO;

public sealed class StreamPipeSink(Stream destination) : IPipeSink
{
    public async Task ReadAsync(Stream pipeStream, CancellationToken ct)
    {
        await pipeStream.CopyToAsync(destination, ct).ConfigureAwait(false);
    }
}
