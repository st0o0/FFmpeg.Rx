using FFmpeg.Rx.IO;

namespace FFmpeg.Rx.Models;

public sealed record PipeInput(
    IPipeSource Source,
    string PipeName,
    string[]? CustomArguments = null) : FfmpegInput(CustomArguments ?? []);
