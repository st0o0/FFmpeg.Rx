using FFmpeg.Rx.IO;

namespace FFmpeg.Rx.Models;

public sealed record PipeOutput(IPipeSink Sink, string PipeName);
