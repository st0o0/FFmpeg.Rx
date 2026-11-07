## Why

FFMpegCore (the current FFmpeg wrapper used in FunkArr) has an unstable API surface with
massive breaking changes between major versions, an impedance mismatch with reactive
architectures (no Observable support), and more abstraction than needed for the primary
use-case (remux, not encode). FFmpeg.Rx replaces it with a purpose-built library modeled
after WebSocket.Rx: R3 Observable event streams, immutable pipeline records, and a fluent
builder — making FFmpeg process interaction composable, testable, and allocation-conscious.

## What Changes

- New NuGet library `FFmpeg.Rx` providing reactive FFmpeg process execution
- Fluent pipeline builder: `Ffmpeg.Input(url).Output(path).Build()` → `FfmpegSession`
- R3 Observable event hierarchy: `StartedEvent`, `ProgressEvent`, `LogEvent`,
  `CompletedEvent`, `FailedEvent` — single `Events` stream with typed convenience
  projections (`.Progress`, `.Log`)
- Immutable record-based pipeline model (inputs, outputs, stream mappings) — builder
  is sugar, records are the testable truth
- Internal process wrapper (no CliWrap dependency) with direct R3 Subject integration
- Built-in error classification (Transient/Permanent) from FFmpeg stderr patterns
- FFmpeg binary discovery: PATH default, explicit `Ffmpeg.At(path)`, Options pattern for DI
- One session = one process. No reuse. Build a new session from the same pipeline for
  another run.

**Not included in this change:**
- DI integration package (Microsoft.Extensions.DependencyInjection) — separate package later
- Filter graph DSL — raw `-vf`/`-af` via `.Arg()`
- Probe/media info (ffprobe) — just process execution
- Encoding presets — consumer builds their own

## Capabilities

### New Capabilities

- `event-stream`: R3 Observable event hierarchy for FFmpeg process lifecycle (Started, Progress, Log, Completed, Failed) with typed convenience projections
- `pipeline-builder`: Fluent API for composing FFmpeg command lines from immutable input/output/mapping records
- `process-runner`: Internal process wrapper that pipes stdout/stderr into R3 Subjects, handles cancellation and exit codes
- `progress-parser`: Parses FFmpeg `-progress pipe:1` stdout blocks into structured ProgressEvent records
- `error-classifier`: Classifies FFmpeg stderr error patterns into Transient/Permanent failure kinds

### Modified Capabilities

(none — greenfield library)

## Impact

- New standalone repo `st0o0/FFmpeg.Rx` with NuGet package
- Single dependency: R3 (Cysharp)
- .NET 10, C# 14
- FunkArr will consume this as a NuGet package, replacing FFMpegCore
  (FunkArr migration is a separate change)
- Project structure: `FFmpeg.Rx` (lib) + `FFmpeg.Rx.Tests` (xUnit v3) +
  `FFmpeg.Rx.Benchmarks` (BenchmarkDotNet, later)
