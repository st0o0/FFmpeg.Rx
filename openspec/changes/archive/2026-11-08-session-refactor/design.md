## Context

FfmpegSession.RunAsync is 100+ lines with two execution paths, duplicated try/catch blocks, and progress logic repeated three times. This refactoring extracts pure functions, eliminates duplication, and bundles state.

## Decisions

### D1: ExecutionMode enum

```csharp
private enum ExecutionMode { EventStream, PipeOutput }
```

Determined once in RunAsync from the PipelineConfig. The single `ExecuteAsync` method uses this to wire CliWrap differently:
- `EventStream`: ListenAsync, stdout for progress, stderr for logs
- `PipeOutput`: ExecuteAsync with PipeTarget, stderr for both progress and logs

### D2: Pure progress emission

```csharp
private static IEnumerable<FfmpegEvent> EmitProgress(
    ProgressEvent progress, TimeSpan? totalDuration)
{
    yield return progress;
    if (totalDuration is { } dur && dur.TotalMicroseconds > 0)
    {
        var percent = Math.Min(100.0, progress.TimeUs / dur.TotalMicroseconds * 100.0);
        yield return new PercentageProgressEvent(percent, progress);
    }
}
```

Called from both stdout and stderr handlers. No duplication.

### D3: Line classifier

```csharp
private static FfmpegEvent? ClassifyStderrLine(
    string line, Dictionary<string, string> progressBlock, bool parseProgress)
{
    if (parseProgress)
    {
        var progress = ProgressParser.ParseLine(line, progressBlock);
        if (progress is not null) return progress;
    }
    return new LogEvent(line.Trim(), ClassifyLogLevel(line));
}
```

In EventStream mode: `parseProgress = false` (progress comes from stdout).
In PipeOutput mode: `parseProgress = true` (progress comes from stderr via -progress pipe:2).

### D4: SessionState aggregator

```csharp
private sealed class SessionState
{
    public readonly List<string> StderrLines = [];
    public readonly Dictionary<string, string> ProgressBlock = [];
    public int ExitCode;
}
```

One object, passed through, no scattered variables.

### D5: Single try/catch

```csharp
try { ... }
catch (OperationCanceledException) { return Cancelled(sw); }
catch (Exception ex) when (ex is not InvalidOperationException) { return StartFailed(sw, ex); }
```

Extracted as helper methods `Cancelled(Stopwatch)` and `StartFailed(Stopwatch, Exception)`.

### D6: C#14 extension block

```csharp
public extension FfmpegSessionExtensions(IFfmpegSession session)
{
    public Observable<double> PercentageProgress(TimeSpan totalDuration) =>
        session.Progress
            .Select(p => Math.Min(100.0, p.TimeUs / totalDuration.TotalMicroseconds * 100.0));
}
```

## Risks

- Refactoring internals while 350 tests exist provides a strong safety net.
- C#14 `extension()` blocks require LangVersion preview or .NET 10+ SDK. Verify compiler support.
