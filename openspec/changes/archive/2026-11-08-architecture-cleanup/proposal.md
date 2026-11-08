## Why

FFmpeg.Rx has two parallel model hierarchies (old string-based and new typed), a ProcessRunner with a dummy-pipeline hack, Named Pipe infrastructure that's Windows-only and overengineered, and duplicated session creation paths. The codebase grew additively — new typed model was added alongside the old instead of replacing it.

## What Changes

Replace the hand-rolled process management with CliWrap, delete the entire old model hierarchy, and collapse the architecture into a clean 3-layer design where FfmpegSession IS the runner.

### Delete old model (12 files)

- `FfmpegPipeline`, `FfmpegOutput`, `StreamMapping` — superseded by `PipelineSpec`, `OutputSpec`, `StreamSpec`
- `FfmpegInput`, `FileInput`, `UrlInput` (old) — superseded by `InputSpec` hierarchy
- `PipeInput`, `PipeOutput` — superseded by `PipeInputSpec`, `PipeDestination`
- `ArgumentBuilder` — superseded by `ArgumentSerializer`
- `OutputOptions`, `InputOptions` (old builders) — superseded by `OutputBuilder`, new typed builders
- `IPipeSource`, `IPipeSink`, `StreamPipeSource`, `StreamPipeSink` — replaced by CliWrap's `PipeSource`/`PipeTarget`

### Replace ProcessRunner with CliWrap

- Delete `ProcessRunner` (226 lines)
- `FfmpegSession.RunAsync()` uses CliWrap directly: `Cli.Wrap(binary).WithArguments(args).ListenAsync()`
- stdout → `ProgressParser` → R3 Subject (unchanged pure function)
- stderr → `ErrorClassifier` + `LogEvent` → R3 Subject
- Pipe I/O via `PipeSource.FromStream` / `PipeTarget.ToStream` — cross-platform, no Named Pipes
- Graceful cancellation: write `q` to stdin pipe, CliWrap's dual-token pattern as fallback

### Simplify ProbeRunner

- Replace manual Process.Start with `Cli.Wrap("ffprobe").ExecuteBufferedAsync()`
- One-liner instead of 40 lines

### Single construction path

- `FfmpegSession` has ONE constructor: `PipelineSpec`
- `FfmpegSession.Create(FfmpegPipeline)` — deleted
- `PipelineBuilder` produces `PipelineSpec` via new typed builders

### Update tests

- All tests already use the new typed API (PipelineSpec + ArgumentSerializer)
- Delete tests that reference old model types
- Integration tests stay as-is (they test behavior, not internals)
