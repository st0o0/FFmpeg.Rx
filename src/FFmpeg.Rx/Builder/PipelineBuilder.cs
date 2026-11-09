using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.Builder;

public sealed class PipelineBuilder
{
    private readonly string? _binaryPath;
    private readonly List<InputConfig> _inputs = [];
    private readonly List<string> _globalOptions = [];
    private TimeSpan? _knownDuration;

    internal PipelineBuilder(string? binaryPath)
    {
        _binaryPath = binaryPath;
    }

    public PipelineBuilder AddInput(InputConfig input)
    {
        _inputs.Add(input);
        return this;
    }

    public PipelineBuilder Input(Uri url)
    {
        _inputs.Add(new UrlInputConfig(url));
        return this;
    }

    public PipelineBuilder Input(Uri url, Action<InputOptionsBuilder> configure)
    {
        var builder = new InputOptionsBuilder();
        configure(builder);
        _inputs.Add(new UrlInputConfig(url) { Options = builder.Build() });
        return this;
    }

    public PipelineBuilder Input(string filePath)
    {
        _inputs.Add(new FileInputConfig(filePath));
        return this;
    }

    public PipelineBuilder Input(string filePath, Action<InputOptionsBuilder> configure)
    {
        var builder = new InputOptionsBuilder();
        configure(builder);
        _inputs.Add(new FileInputConfig(filePath) { Options = builder.Build() });
        return this;
    }

    public PipelineBuilder Input(Stream source, string format)
    {
        _inputs.Add(new PipeInputConfig(source, format));
        return this;
    }

    public PipelineBuilder Input(Stream source, string format, Action<InputOptionsBuilder> configure)
    {
        var builder = new InputOptionsBuilder();
        configure(builder);
        _inputs.Add(new PipeInputConfig(source, format) { Options = builder.Build() });
        return this;
    }

    public PipelineBuilder WithProgressReporting(TimeSpan? knownDuration = null)
    {
        _globalOptions.Add("-progress pipe:1");
        _knownDuration = knownDuration;
        return this;
    }

    public IFfmpegSession Output(string path, Action<OutputBuilder> configure)
    {
        var builder = new OutputBuilder(new FileDestination(path));
        configure(builder);
        return BuildSession([builder.Build()]);
    }

    public IFfmpegSession OutputPipe(Stream destination, Action<OutputBuilder> configure)
    {
        var builder = new OutputBuilder(new PipeDestination(destination));
        configure(builder);
        return BuildSession([builder.Build()]);
    }

    public IFfmpegSession OutputNull(Action<OutputBuilder> configure)
    {
        var builder = new OutputBuilder(NullDestination.Instance);
        configure(builder);
        var output = builder.Build();
        output = output with { Container = Codecs.ContainerFormat.Null };
        return BuildSession([output]);
    }

    public IFfmpegSession OutputTee(Action<OutputBuilder> configureOutput, Action<TeeBuilder> configureTee)
    {
        var outputBuilder = new OutputBuilder(new FileDestination(""));
        configureOutput(outputBuilder);
        var teeBuilder = new TeeBuilder();
        configureTee(teeBuilder);
        var output = outputBuilder.Build() with { Tee = teeBuilder.Build() };
        return BuildSession([output]);
    }

    public MultiOutputBuilder Outputs() => new(_inputs, _globalOptions, _binaryPath);

    private IFfmpegSession BuildSession(OutputConfig[] outputs)
    {
        var spec = new PipelineConfig(
            [.. _inputs],
            outputs,
            _globalOptions.Count > 0 ? [.. _globalOptions] : null,
            _binaryPath,
            _knownDuration);
        return FfmpegSession.Create(spec);
    }
}
