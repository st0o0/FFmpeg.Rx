namespace FFmpeg.Rx.Models;

public abstract record OutputDestination;

public sealed record FileDestination(string Path) : OutputDestination;

public sealed record PipeDestination(Stream Destination) : OutputDestination;

public sealed record NullDestination : OutputDestination
{
    public static NullDestination Instance { get; } = new();
}
