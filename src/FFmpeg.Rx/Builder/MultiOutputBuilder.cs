using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.Builder;

public sealed class MultiOutputBuilder
{
    private readonly List<FfmpegInput> _inputs;
    private readonly List<string> _globalOptions;
    private readonly string? _binaryPath;
    private readonly List<FfmpegOutput> _outputs = [];

    internal MultiOutputBuilder(List<FfmpegInput> inputs, List<string> globalOptions, string? binaryPath)
    {
        _inputs = inputs;
        _globalOptions = globalOptions;
        _binaryPath = binaryPath;
    }

    public MultiOutputBuilder Output(string path, Action<OutputOptions> configure)
    {
        var options = new OutputOptions();
        configure(options);
        _outputs.Add(new FfmpegOutput(
            path, [.. options.Mappings], [.. options.Arguments], options.SuppressOverwrite,
            options.VideoFilters, options.AudioFilters, options.FilterComplexString,
            options.LabelMappings.Count > 0 ? [.. options.LabelMappings] : null));
        return this;
    }

    public FfmpegSession Build()
    {
        var pipeline = new FfmpegPipeline(
            [.. _inputs], [.. _outputs], [.. _globalOptions], _binaryPath);
        return FfmpegSession.Create(pipeline);
    }
}
