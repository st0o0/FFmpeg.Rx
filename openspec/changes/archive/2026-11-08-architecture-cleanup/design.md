## Context

FFmpeg.Rx grew additively: v1-roadmap built a string-based model, api-redesign added a typed model alongside it. The result is two parallel worlds with hacks to bridge them. This change deletes the old world and rebuilds the execution layer on CliWrap.

## Decisions

### D1: CliWrap as process abstraction

Add `CliWrap` NuGet dependency. It provides:
- Deadlock-free stdin/stdout/stderr handling
- `PipeSource.FromStream` / `PipeTarget.ToStream` for pipe I/O (cross-platform)
- `ListenAsync()` → `IAsyncEnumerable<CommandEvent>` for real-time output
- Immutable `Command` building (matches our immutable PipelineSpec)
- Dual-token cancellation (graceful + forceful)

**Trade-off:** +1 dependency, but -12 files and -300 lines of manual process code.

**Alternative considered:** Wait for .NET 11's built-in Process API improvements. Rejected — .NET 11 isn't released yet, and CliWrap works on .NET 10 today. Can re-evaluate when .NET 11 ships.

**Alternative considered:** Keep manual Process handling, just clean it up. Rejected — the Named Pipe system, deadlock prevention, and cross-platform concerns are exactly what CliWrap solves. Reimplementing them is wasted effort.

### D2: FfmpegSession IS the runner

No separate ProcessRunner class. FfmpegSession.RunAsync() directly builds and executes the CliWrap command:

```csharp
public async Task<FfmpegResult> RunAsync(CancellationToken ct = default)
{
    var cmd = Cli.Wrap(_binaryPath)
        .WithArguments(_args)
        .WithValidation(CommandResultValidation.None);

    // Pipe I/O if needed
    // ListenAsync → parse progress, collect stderr
    // Bridge to R3 Subject
    // Build FfmpegResult
}
```

The session owns the full lifecycle: build command → execute → parse → emit events → return result.

### D3: Pipe I/O via CliWrap PipeSource/PipeTarget

Input piping: `PipeInputSpec` carries a `Stream` reference. Session sets `WithStandardInputPipe(PipeSource.FromStream(stream))` and uses `-i pipe:0` in args.

Output piping: `PipeDestination` carries a `Stream` reference. Session sets `WithStandardOutputPipe(PipeTarget.ToStream(stream))` and uses `pipe:1` as output path.

No Named Pipes, no pipe servers, no Windows-only guards. Works on all platforms.

**Note:** When stdin is used for pipe input, graceful `q` shutdown goes through CliWrap's cancellation instead. FFmpeg responds to SIGINT (Unix) and Ctrl+C (Windows) which CliWrap sends on graceful cancel.

### D4: Graceful shutdown strategy

Three cases:
1. **No stdin piping:** Write `q` to stdin before cancelling (current behavior, works)
2. **Stdin piping:** Can't write `q` — use CliWrap's graceful cancellation (sends Ctrl+C/SIGINT)
3. **Fallback:** CliWrap's forceful token kills the process after timeout

CliWrap's `ExecuteAsync(gracefulCt, forcefulCt)` pattern handles this.

### D5: Delete old model completely

All old types are deleted. No backward compatibility shims. The library has no published consumers yet.

Files deleted:
- `Models/FfmpegPipeline.cs` (FfmpegPipeline, FfmpegInput, UrlInput, FileInput, TeeOutput, TeeTarget)
- `Models/FfmpegOutput.cs` (FfmpegOutput, StreamMapping)
- `Models/PipeInput.cs`
- `Models/PipeOutput.cs`
- `Internal/ProcessRunner.cs`
- `Internal/ArgumentBuilder.cs`
- `Builder/OutputOptions.cs`
- `Builder/InputOptions.cs`
- `IO/IPipeSource.cs`
- `IO/IPipeSink.cs`
- `IO/StreamPipeSource.cs`
- `IO/StreamPipeSink.cs`

### D6: PipelineBuilder rewired

PipelineBuilder collects `InputSpec` (not `FfmpegInput`) and produces `PipelineSpec` (not `FfmpegPipeline`). Output methods use `OutputBuilder` (not `OutputOptions`). The fluent API surface stays the same for consumers — only the internal wiring changes.

### D7: ProbeRunner simplified

Replace manual Process.Start + ReadToEndAsync with:
```csharp
var result = await Cli.Wrap(ffprobePath)
    .WithArguments(args)
    .ExecuteBufferedAsync(ct);
return ProbeResultParser.Parse(result.StandardOutput);
```

## Risks

- **CliWrap version churn** — Pin to a major version, don't chase every update
- **Graceful shutdown behavior** — FFmpeg may not respond identically to Ctrl+C vs `q` — test thoroughly
- **CliWrap's IObservable is System.Reactive** — We don't use it, we use ListenAsync (IAsyncEnumerable) and bridge to R3 ourselves
