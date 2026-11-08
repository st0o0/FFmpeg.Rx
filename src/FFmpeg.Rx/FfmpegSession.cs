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

        var totalDuration = await ResolveDuration(ct).ConfigureAwait(false);
        var mode = DetectMode(_spec);
        var state = new SessionState();
        var sw = Stopwatch.StartNew();

        var cmd = BuildCommand(mode, state, totalDuration);

        try
        {
            if (mode == ExecutionMode.PipeOutput)
            {
                _subject.OnNext(new StartedEvent(0, $"{_binaryPath} {_args}"));
                var result = await cmd.ExecuteAsync(ct).ConfigureAwait(false);
                state.ExitCode = result.ExitCode;
            }
            else
            {
                await foreach (var cmdEvent in cmd.ListenAsync(ct).ConfigureAwait(false))
                {
                    switch (cmdEvent)
                    {
                        case StartedCommandEvent started:
                            _subject.OnNext(new StartedEvent(started.ProcessId, $"{_binaryPath} {_args}"));
                            break;
                        case StandardOutputCommandEvent stdOut:
                            HandleStdoutLine(stdOut.Text, state, totalDuration);
                            break;
                        case StandardErrorCommandEvent stdErr:
                            HandleStderrLine(stdErr.Text, state, mode, totalDuration);
                            break;
                        case ExitedCommandEvent exited:
                            state.ExitCode = exited.ExitCode;
                            break;
                    }
                }
            }

            sw.Stop();
            return BuildResult(state, sw.Elapsed);
        }
        catch (OperationCanceledException)
        {
            return HandleCancellation(sw);
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            return HandleStartFailure(sw, ex);
        }
    }

    public static IFfmpegSession Create(PipelineSpec spec) => new FfmpegSession(spec);

    public ValueTask DisposeAsync()
    {
        _subject.Dispose();
        return ValueTask.CompletedTask;
    }

    private async Task<TimeSpan?> ResolveDuration(CancellationToken ct)
    {
        if (_spec.TotalDuration is not null)
        {
            return _spec.TotalDuration;
        }

        if (_spec.Inputs is [FileInputSpec fileInput])
        {
            try
            {
                var info = await Ffprobe.AnalyseAsync(fileInput.Path, ct).ConfigureAwait(false);
                return info.Duration;
            }
            catch
            {
                return null;
            }
        }

        return null;
    }

    private Command BuildCommand(ExecutionMode mode, SessionState state, TimeSpan? totalDuration)
    {
        var cmd = Cli.Wrap(_binaryPath)
            .WithArguments(_args)
            .WithValidation(CommandResultValidation.None);

        var pipeInput = _spec.Inputs.OfType<PipeInputSpec>().FirstOrDefault();
        if (pipeInput is not null)
        {
            cmd = cmd.WithStandardInputPipe(PipeSource.FromStream(pipeInput.Source));
        }

        if (mode == ExecutionMode.PipeOutput)
        {
            var pipeOutput = _spec.Outputs
                .Select(o => o.Destination)
                .OfType<PipeDestination>()
                .First();

            cmd = cmd
                .WithStandardOutputPipe(PipeTarget.ToStream(pipeOutput.Destination))
                .WithStandardErrorPipe(PipeTarget.ToDelegate(line =>
                    HandleStderrLine(line, state, mode, totalDuration)));
        }

        return cmd;
    }

    private void HandleStdoutLine(string line, SessionState state, TimeSpan? totalDuration)
    {
        var progress = ProgressParser.ParseLine(line, state.ProgressBlock);
        if (progress is not null)
        {
            foreach (var evt in EmitProgress(progress, totalDuration))
            {
                _subject.OnNext(evt);
            }
        }
    }

    private void HandleStderrLine(string line, SessionState state, ExecutionMode mode, TimeSpan? totalDuration)
    {
        if (mode == ExecutionMode.PipeOutput)
        {
            var progress = ProgressParser.ParseLine(line, state.ProgressBlock);
            if (progress is not null)
            {
                foreach (var evt in EmitProgress(progress, totalDuration))
                {
                    _subject.OnNext(evt);
                }

                return;
            }
        }

        state.StderrLines.Add(line);
        _subject.OnNext(new LogEvent(line.Trim(), ClassifyLogLevel(line)));
    }

    internal static IEnumerable<FfmpegEvent> EmitProgress(ProgressEvent progress, TimeSpan? totalDuration)
    {
        yield return progress;
        if (totalDuration is { } dur && dur.TotalMicroseconds > 0)
        {
            var percent = Math.Min(100.0, progress.TimeUs / dur.TotalMicroseconds * 100.0);
            yield return new PercentageProgressEvent(percent, progress);
        }
    }

    private FfmpegResult BuildResult(SessionState state, TimeSpan elapsed)
    {
        if (state.ExitCode == 0)
        {
            _subject.OnNext(new CompletedEvent(0, elapsed));
            _subject.OnCompleted();
            return new FfmpegResult(true, 0, null, elapsed);
        }

        var stderr = string.Join('\n', state.StderrLines);
        var error = ErrorClassifier.ExtractError(stderr);
        var kind = ErrorClassifier.Classify(error);
        _subject.OnNext(new FailedEvent(state.ExitCode, error, kind));
        _subject.OnCompleted();
        return new FfmpegResult(false, state.ExitCode, error, elapsed, kind);
    }

    private FfmpegResult HandleCancellation(Stopwatch sw)
    {
        sw.Stop();
        _subject.OnNext(new FailedEvent(-1, "Cancelled", FailureKind.Unknown));
        _subject.OnCompleted();
        return new FfmpegResult(false, -1, "Cancelled", sw.Elapsed);
    }

    private FfmpegResult HandleStartFailure(Stopwatch sw, Exception ex)
    {
        sw.Stop();
        var error = $"Failed to start FFmpeg: {ex.Message}";
        _subject.OnNext(new FailedEvent(1, error, FailureKind.Permanent));
        _subject.OnCompleted();
        return new FfmpegResult(false, 1, error, sw.Elapsed, FailureKind.Permanent);
    }

    private static ExecutionMode DetectMode(PipelineSpec spec) =>
        spec.Outputs.Any(o => o.Destination is PipeDestination)
            ? ExecutionMode.PipeOutput
            : ExecutionMode.EventStream;

    internal static LogLevel ClassifyLogLevel(string line)
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

    private enum ExecutionMode { EventStream, PipeOutput }

    private sealed class SessionState
    {
        public readonly List<string> StderrLines = [];
        public readonly Dictionary<string, string> ProgressBlock = [];
        public int ExitCode;
    }
}
