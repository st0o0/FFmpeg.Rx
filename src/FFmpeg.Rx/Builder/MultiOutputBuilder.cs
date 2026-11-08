using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.Builder;

public sealed class MultiOutputBuilder
{
    private readonly List<InputConfig> _inputs;
    private readonly List<string> _globalOptions;
    private readonly string? _binaryPath;
    private readonly List<OutputConfig> _outputs = [];

    internal MultiOutputBuilder(List<InputConfig> inputs, List<string> globalOptions, string? binaryPath)
    {
        _inputs = inputs;
        _globalOptions = globalOptions;
        _binaryPath = binaryPath;
    }

    public MultiOutputBuilder Output(string path, Action<OutputBuilder> configure)
    {
        var builder = new OutputBuilder(new FileDestination(path));
        configure(builder);
        _outputs.Add(builder.Build());
        return this;
    }

    public IFfmpegSession Build()
    {
        var spec = new PipelineConfig(
            [.. _inputs],
            [.. _outputs],
            _globalOptions.Count > 0 ? [.. _globalOptions] : null,
            _binaryPath);
        return FfmpegSession.Create(spec);
    }
}
