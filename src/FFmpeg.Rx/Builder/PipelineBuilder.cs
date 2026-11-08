using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.Builder;

public sealed class PipelineBuilder
{
    private readonly string? _binaryPath;
    private readonly List<InputSpec> _inputs = [];
    private readonly List<string> _globalOptions = [];

    internal PipelineBuilder(string? binaryPath)
    {
        _binaryPath = binaryPath;
    }

    internal PipelineBuilder AddInput(InputSpec input)
    {
        _inputs.Add(input);
        return this;
    }

    public PipelineBuilder Input(Uri url)
    {
        _inputs.Add(new UrlInputSpec(url));
        return this;
    }

    public PipelineBuilder Input(string filePath)
    {
        _inputs.Add(new FileInputSpec(filePath));
        return this;
    }

    public PipelineBuilder Input(Stream source, string format)
    {
        _inputs.Add(new PipeInputSpec(source, format));
        return this;
    }

    public PipelineBuilder WithProgressReporting()
    {
        _globalOptions.Add("-progress pipe:1");
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

    public MultiOutputBuilder Outputs()
    {
        return new MultiOutputBuilder(_inputs, _globalOptions, _binaryPath);
    }

    private IFfmpegSession BuildSession(OutputSpec[] outputs)
    {
        var spec = new PipelineSpec(
            [.. _inputs],
            outputs,
            _globalOptions.Count > 0 ? [.. _globalOptions] : null,
            _binaryPath);
        return FfmpegSession.Create(spec);
    }
}
