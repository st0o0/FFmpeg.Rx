using FFmpeg.Rx.IO;

namespace FFmpeg.Rx.Models;

public abstract record OutputDestination;

public sealed record FileDestination(string Path) : OutputDestination;

public sealed record PipeDestination(IPipeSink Sink, string PipeName) : OutputDestination;

public sealed record NullDestination : OutputDestination
{
    public static NullDestination Instance { get; } = new();
}
