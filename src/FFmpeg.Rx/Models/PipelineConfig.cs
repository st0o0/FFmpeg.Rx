namespace FFmpeg.Rx.Models;

public sealed record PipelineConfig(
    IReadOnlyList<InputConfig> Inputs,
    IReadOnlyList<OutputConfig> Outputs,
    IReadOnlyList<string>? GlobalOptions = null,
    string? BinaryPath = null,
    TimeSpan? TotalDuration = null)
{
    public OutputConfig Output => Outputs[0];
}
