using System.Diagnostics;
using CliWrap;
using CliWrap.EventStream;
using FFmpeg.Rx.Events;
using FFmpeg.Rx.Internal;
using FFmpeg.Rx.Models;
using R3;

namespace FFmpeg.Rx;

public sealed class FfmpegSession : IFfmpegSession
{
    private readonly PipelineSpec _spec;
    private readonly string _args;
    private readonly string _binaryPath;
    private readonly Subject<FfmpegEvent> _subject = new();
    private int _started;

    private FfmpegSession(PipelineSpec spec)
    {
        _spec = spec;
        _args = ArgumentSerializer.Serialize(spec);
        _binaryPath = FfmpegOptions.ResolveFfmpegBinary(spec.BinaryPath);
    }

    public Observable<FfmpegEvent> Events => _subject.AsObservable();

    public Observable<ProgressEvent> Progress => Events.OfType<FfmpegEvent, ProgressEvent>();

    public Observable<LogEvent> Log => Events.OfType<FfmpegEvent, LogEvent>();

    public async Task<FfmpegResult> RunAsync(CancellationToken ct = default)
    {
        if (Interlocked.Exchange(ref _started, 1) != 0)
        {
            throw new InvalidOperationException(
                "This session has already been run. Build a new session from the pipeline.");
        }

        var sw = Stopwatch.StartNew();
        var stderrLines = new List<string>();
        var progressBlock = new Dictionary<string, string>();

        var cmd = Cli.Wrap(_binaryPath)
            .WithArguments(_args)
            .WithValidation(CommandResultValidation.None);

        var pipeInput = _spec.Inputs.OfType<PipeInputSpec>().FirstOrDefault();
        if (pipeInput is not null)
        {
            cmd = cmd.WithStandardInputPipe(PipeSource.FromStream(pipeInput.Source));
        }

        var pipeOutput = _spec.Outputs
            .Select(o => o.Destination)
            .OfType<PipeDestination>()
            .FirstOrDefault();

        if (pipeOutput is not null)
        {
            cmd = cmd
                .WithStandardOutputPipe(PipeTarget.ToStream(pipeOutput.Destination))
                .WithStandardErrorPipe(PipeTarget.ToDelegate(line =>
                {
                    stderrLines.Add(line);
                    _subject.OnNext(new LogEvent(line.Trim(), ClassifyLogLevel(line)));
                }));

            return await ExecuteWithPipeOutput(cmd, sw, stderrLines, ct).ConfigureAwait(false);
        }

        return await ExecuteWithEventStream(cmd, sw, stderrLines, progressBlock, ct).ConfigureAwait(false);
    }

    private async Task<FfmpegResult> ExecuteWithEventStream(
        Command cmd,
        Stopwatch sw,
        List<string> stderrLines,
        Dictionary<string, string> progressBlock,
        CancellationToken ct)
    {
        var exitCode = 0;

        try
        {
            await foreach (var cmdEvent in cmd.ListenAsync(ct).ConfigureAwait(false))
            {
                switch (cmdEvent)
                {
                    case StartedCommandEvent started:
                        _subject.OnNext(new StartedEvent(started.ProcessId, $"{_binaryPath} {_args}"));
                        break;
                    case StandardOutputCommandEvent stdOut:
                        var progress = ProgressParser.ParseLine(stdOut.Text, progressBlock);
                        if (progress is not null)
                        {
                            _subject.OnNext(progress);
                        }

                        break;
                    case StandardErrorCommandEvent stdErr:
                        stderrLines.Add(stdErr.Text);
                        _subject.OnNext(new LogEvent(stdErr.Text.Trim(), ClassifyLogLevel(stdErr.Text)));
                        break;
                    case ExitedCommandEvent exited:
                        exitCode = exited.ExitCode;
                        break;
                }
            }

            sw.Stop();
            return BuildResult(exitCode, stderrLines, sw.Elapsed);
        }
        catch (OperationCanceledException)
        {
            sw.Stop();
            _subject.OnNext(new FailedEvent(-1, "Cancelled", FailureKind.Unknown));
            _subject.OnCompleted();
            return new FfmpegResult(false, -1, "Cancelled", sw.Elapsed);
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            sw.Stop();
            var error = $"Failed to start FFmpeg: {ex.Message}";
            _subject.OnNext(new FailedEvent(1, error, FailureKind.Permanent));
            _subject.OnCompleted();
            return new FfmpegResult(false, 1, error, sw.Elapsed, FailureKind.Permanent);
        }
    }

    private async Task<FfmpegResult> ExecuteWithPipeOutput(
        Command cmd,
        Stopwatch sw,
        List<string> stderrLines,
        CancellationToken ct)
    {
        try
        {
            _subject.OnNext(new StartedEvent(0, $"{_binaryPath} {_args}"));

            var result = await cmd.ExecuteAsync(ct).ConfigureAwait(false);
            sw.Stop();

            return BuildResult(result.ExitCode, stderrLines, sw.Elapsed);
        }
        catch (OperationCanceledException)
        {
            sw.Stop();
            _subject.OnNext(new FailedEvent(-1, "Cancelled", FailureKind.Unknown));
            _subject.OnCompleted();
            return new FfmpegResult(false, -1, "Cancelled", sw.Elapsed);
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            sw.Stop();
            var error = $"Failed to start FFmpeg: {ex.Message}";
            _subject.OnNext(new FailedEvent(1, error, FailureKind.Permanent));
            _subject.OnCompleted();
            return new FfmpegResult(false, 1, error, sw.Elapsed, FailureKind.Permanent);
        }
    }

    private FfmpegResult BuildResult(int exitCode, List<string> stderrLines, TimeSpan elapsed)
    {
        if (exitCode == 0)
        {
            _subject.OnNext(new CompletedEvent(0, elapsed));
            _subject.OnCompleted();
            return new FfmpegResult(true, 0, null, elapsed);
        }

        var stderr = string.Join('\n', stderrLines);
        var error = ErrorClassifier.ExtractError(stderr);
        var kind = ErrorClassifier.Classify(error);
        _subject.OnNext(new FailedEvent(exitCode, error, kind));
        _subject.OnCompleted();
        return new FfmpegResult(false, exitCode, error, elapsed, kind);
    }

    public static IFfmpegSession Create(PipelineSpec spec)
    {
        return new FfmpegSession(spec);
    }

    public ValueTask DisposeAsync()
    {
        _subject.Dispose();
        return ValueTask.CompletedTask;
    }

    private static LogLevel ClassifyLogLevel(string line)
    {
        if (line.Contains("[error]", StringComparison.OrdinalIgnoreCase) ||
            line.Contains("Error", StringComparison.OrdinalIgnoreCase))
        {
            return LogLevel.Error;
        }

        if (line.Contains("[warning]", StringComparison.OrdinalIgnoreCase))
        {
            return LogLevel.Warning;
        }

        if (line.Contains("[fatal]", StringComparison.OrdinalIgnoreCase))
        {
            return LogLevel.Fatal;
        }

        return LogLevel.Info;
    }
}
