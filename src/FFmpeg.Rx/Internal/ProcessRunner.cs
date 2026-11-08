using System.Diagnostics;
using System.IO.Pipes;
using FFmpeg.Rx.Events;
using FFmpeg.Rx.Models;
using R3;

namespace FFmpeg.Rx.Internal;

internal sealed class ProcessRunner
{
    private readonly FfmpegPipeline _pipeline;
    private readonly string _binaryPath;
    private readonly string? _prebuiltArgs;
    private readonly Subject<FfmpegEvent> _events;
    private Process? _process;

    internal ProcessRunner(FfmpegPipeline pipeline, Subject<FfmpegEvent> events)
    {
        _pipeline = pipeline;
        _binaryPath = FfmpegOptions.ResolveFfmpegBinary(pipeline.BinaryPath);
        _events = events;
    }

    internal ProcessRunner(FfmpegPipeline pipeline, Subject<FfmpegEvent> events, string prebuiltArgs)
    {
        _pipeline = pipeline;
        _binaryPath = FfmpegOptions.ResolveFfmpegBinary(pipeline.BinaryPath);
        _prebuiltArgs = prebuiltArgs;
        _events = events;
    }

    internal async Task<FfmpegResult> RunAsync(CancellationToken ct)
    {
        var arguments = _prebuiltArgs ?? ArgumentBuilder.Build(_pipeline);
        var sw = Stopwatch.StartNew();

        var psi = new ProcessStartInfo
        {
            FileName = _binaryPath,
            Arguments = arguments,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            RedirectStandardInput = true
        };

        var workDir = FfmpegOptions.Global?.WorkingDirectory;
        if (workDir is not null)
        {
            psi.WorkingDirectory = workDir;
        }

        try
        {
            _process = Process.Start(psi)
                ?? throw new InvalidOperationException($"Failed to start process: {_binaryPath}");
        }
        catch (Exception ex) when (ex is System.ComponentModel.Win32Exception or InvalidOperationException)
        {
            sw.Stop();
            var error = $"Failed to start FFmpeg: {ex.Message}";
            _events.OnNext(new FailedEvent(1, error, FailureKind.Permanent));
            _events.OnCompleted();
            return new FfmpegResult(false, 1, error, sw.Elapsed, FailureKind.Permanent);
        }

        _events.OnNext(new StartedEvent(_process.Id, $"{_binaryPath} {arguments}"));

        using var registration = ct.Register(() => GracefulShutdown(_process));

        var pipeTasks = new List<Task>();
        StartPipeOperations(pipeTasks, ct);

        var progressBlock = new Dictionary<string, string>();
        var stderrTask = _process.StandardError.ReadToEndAsync(ct);

        while (await _process.StandardOutput.ReadLineAsync(ct).ConfigureAwait(false) is { } line)
        {
            var progress = ProgressParser.ParseLine(line, progressBlock);
            if (progress is not null)
            {
                _events.OnNext(progress);
            }
        }

        if (pipeTasks.Count > 0)
        {
            try { await Task.WhenAll(pipeTasks).ConfigureAwait(false); } catch { }
        }

        await _process.WaitForExitAsync(ct).ConfigureAwait(false);
        sw.Stop();

        string stderr;
        try
        {
            stderr = await stderrTask.ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            stderr = "";
        }

        if (ct.IsCancellationRequested)
        {
            _events.OnNext(new FailedEvent(-1, "Cancelled", FailureKind.Unknown));
            _events.OnCompleted();
            return new FfmpegResult(false, -1, "Cancelled", sw.Elapsed);
        }

        if (!string.IsNullOrWhiteSpace(stderr))
        {
            foreach (var logLine in stderr.Split('\n', StringSplitOptions.RemoveEmptyEntries))
            {
                _events.OnNext(new LogEvent(logLine.Trim(), ClassifyLogLevel(logLine)));
            }
        }

        if (_process.ExitCode == 0)
        {
            _events.OnNext(new CompletedEvent(0, sw.Elapsed));
            _events.OnCompleted();
            return new FfmpegResult(true, 0, null, sw.Elapsed);
        }

        var error2 = ErrorClassifier.ExtractError(stderr);
        var kind = ErrorClassifier.Classify(error2);
        _events.OnNext(new FailedEvent(_process.ExitCode, error2, kind));
        _events.OnCompleted();
        return new FfmpegResult(false, _process.ExitCode, error2, sw.Elapsed, kind);
    }

    internal void Kill()
    {
        try
        {
            if (_process is { HasExited: false })
            {
                _process.Kill(entireProcessTree: true);
            }
        }
        catch
        {
            // already exited
        }
    }

    private void StartPipeOperations(List<Task> tasks, CancellationToken ct)
    {
        foreach (var input in _pipeline.Inputs)
        {
            if (input is PipeInput pipeInput && OperatingSystem.IsWindows())
            {
                tasks.Add(Task.Run(async () =>
                {
                    var server = new NamedPipeServerStream(
                        pipeInput.PipeName, PipeDirection.Out, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
                    await using (server.ConfigureAwait(false))
                    {
                        await server.WaitForConnectionAsync(ct).ConfigureAwait(false);
                        await pipeInput.Source.WriteAsync(server, ct).ConfigureAwait(false);
                    }
                }, ct));
            }
        }

        if (_pipeline.PipeOutput is { } pipeOutput && OperatingSystem.IsWindows())
        {
            tasks.Add(Task.Run(async () =>
            {
                var server = new NamedPipeServerStream(
                    pipeOutput.PipeName, PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
                await using (server.ConfigureAwait(false))
                {
                    await server.WaitForConnectionAsync(ct).ConfigureAwait(false);
                    await pipeOutput.Sink.ReadAsync(server, ct).ConfigureAwait(false);
                }
            }, ct));
        }
    }

    private static void GracefulShutdown(Process process)
    {
        try
        {
            if (process.HasExited)
            {
                return;
            }

            process.StandardInput.Write('q');
            process.StandardInput.Flush();

            if (!process.WaitForExit(5000))
            {
                process.Kill(entireProcessTree: true);
            }
        }
        catch
        {
            try { process.Kill(entireProcessTree: true); } catch { }
        }
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
