using FFmpeg.Rx.IO;
using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.Builder;

public sealed class PipelineBuilder
{
    private readonly string? _binaryPath;
    private readonly List<FfmpegInput> _inputs = [];
    private readonly List<string> _globalOptions = [];

    internal PipelineBuilder(string? binaryPath)
    {
        _binaryPath = binaryPath;
    }

    internal PipelineBuilder AddInput(FfmpegInput input)
    {
        _inputs.Add(input);
        return this;
    }

    public PipelineBuilder Input(Uri url, Action<InputOptions>? configure = null)
    {
        var options = new InputOptions();
        configure?.Invoke(options);
        _inputs.Add(new UrlInput(url, options.UserAgentValue, options.ProxyValue, options.Arguments));
        return this;
    }

    public PipelineBuilder Input(string filePath, Action<InputOptions>? configure = null)
    {
        var options = new InputOptions();
        configure?.Invoke(options);
        _inputs.Add(new FileInput(filePath, options.Arguments));
        return this;
    }

    public PipelineBuilder Input(IPipeSource source, Action<InputOptions>? configure = null)
    {
        var options = new InputOptions();
        configure?.Invoke(options);
        var pipeName = $"ffmpeg_rx_{Guid.NewGuid():N}";
        _inputs.Add(new PipeInput(source, pipeName, options.Arguments));
        return this;
    }

    public PipelineBuilder WithProgressReporting()
    {
        _globalOptions.Add("-progress pipe:1");
        return this;
    }

    public IFfmpegSession Output(string path, Action<OutputOptions> configure)
    {
        var options = new OutputOptions();
        configure(options);
        var output = new FfmpegOutput(
            path, [.. options.Mappings], [.. options.Arguments], options.SuppressOverwrite,
            options.VideoFilters, options.AudioFilters, options.FilterComplexString,
            options.LabelMappings.Count > 0 ? [.. options.LabelMappings] : null);
        var pipeline = new FfmpegPipeline([.. _inputs], output, [.. _globalOptions], _binaryPath);
        return FfmpegSession.Create(pipeline);
    }

    public IFfmpegSession OutputPipe(IPipeSink sink, Action<OutputOptions> configure)
    {
        var options = new OutputOptions();
        configure(options);
        var pipeName = $"ffmpeg_rx_{Guid.NewGuid():N}";
        var pipeOutput = new PipeOutput(sink, pipeName);
        var output = new FfmpegOutput(
            GetPipePath(pipeName), [.. options.Mappings], [.. options.Arguments], options.SuppressOverwrite,
            options.VideoFilters, options.AudioFilters, options.FilterComplexString,
            options.LabelMappings.Count > 0 ? [.. options.LabelMappings] : null);
        var pipeline = new FfmpegPipeline([.. _inputs], output, [.. _globalOptions], _binaryPath, pipeOutput);
        return FfmpegSession.Create(pipeline);
    }

    public MultiOutputBuilder Outputs()
    {
        return new MultiOutputBuilder(_inputs, _globalOptions, _binaryPath);
    }

    public IFfmpegSession OutputTee(Action<OutputOptions> configureOutput, Action<TeeBuilder> configureTee)
    {
        var options = new OutputOptions();
        configureOutput(options);
        var teeBuilder = new TeeBuilder();
        configureTee(teeBuilder);
        var output = new FfmpegOutput(
            "", [.. options.Mappings], [.. options.Arguments], false,
            options.VideoFilters, options.AudioFilters, options.FilterComplexString,
            options.LabelMappings.Count > 0 ? [.. options.LabelMappings] : null);
        var pipeline = new FfmpegPipeline(
            [.. _inputs], [output], [.. _globalOptions], _binaryPath, null, teeBuilder.Build());
        return FfmpegSession.Create(pipeline);
    }

    public IFfmpegSession OutputNull(Action<OutputOptions> configure)
    {
        var options = new OutputOptions();
        configure(options);
        options.ForceFormat("null");
        var output = new FfmpegOutput(
            "-", [.. options.Mappings], [.. options.Arguments], options.SuppressOverwrite,
            options.VideoFilters, options.AudioFilters, options.FilterComplexString,
            options.LabelMappings.Count > 0 ? [.. options.LabelMappings] : null);
        var pipeline = new FfmpegPipeline([.. _inputs], output, [.. _globalOptions], _binaryPath);
        return FfmpegSession.Create(pipeline);
    }

    private static string GetPipePath(string pipeName)
    {
        if (OperatingSystem.IsWindows())
        {
            return $@"\\.\pipe\{pipeName}";
        }
        return $"pipe:{pipeName}";
    }
}
