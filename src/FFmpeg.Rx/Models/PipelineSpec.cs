namespace FFmpeg.Rx.Models;

public sealed record PipelineSpec(
    IReadOnlyList<InputSpec> Inputs,
    IReadOnlyList<OutputSpec> Outputs,
    IReadOnlyList<string>? GlobalOptions = null,
    string? BinaryPath = null,
    TimeSpan? TotalDuration = null)
{
    public OutputSpec Output => Outputs[0];
}
