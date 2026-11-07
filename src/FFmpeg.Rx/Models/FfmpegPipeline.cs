namespace FFmpeg.Rx.Models;

public sealed record FfmpegPipeline(
    FfmpegInput[] Inputs,
    FfmpegOutput[] Outputs,
    string[] GlobalOptions,
    string? BinaryPath = null,
    PipeOutput? PipeOutput = null,
    TeeOutput? TeeOutput = null)
{
    public FfmpegOutput Output => Outputs[0];

    public FfmpegPipeline(
        FfmpegInput[] Inputs,
        FfmpegOutput Output,
        string[] GlobalOptions,
        string? BinaryPath = null,
        PipeOutput? PipeOutput = null)
        : this(Inputs, [Output], GlobalOptions, BinaryPath, PipeOutput, null)
    {
    }
}

public abstract record FfmpegInput(string[] CustomArguments);

public sealed record UrlInput(
    Uri Url,
    string? UserAgent = null,
    string? Proxy = null,
    string[]? CustomArguments = null) : FfmpegInput(CustomArguments ?? []);

public sealed record FileInput(
    string Path,
    string[]? CustomArguments = null) : FfmpegInput(CustomArguments ?? []);

public sealed record TeeOutput(TeeTarget[] Targets);

public sealed record TeeTarget(
    string Path,
    string? Format = null,
    string? Select = null,
    string? BitstreamFilter = null,
    string? MovFlags = null,
    bool IgnoreFailure = false);
