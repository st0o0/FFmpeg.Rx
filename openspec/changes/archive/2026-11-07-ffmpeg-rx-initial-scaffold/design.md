## Context

FFmpeg.Rx is a greenfield library in a new repo. The template repo (`st0o0/FFmpeg.Rx`)
is scaffolded from the standard `ProjectName` template with CI, release-please, renovate,
and .NET 10 + xUnit v3. The placeholder `ProjectName` projects need renaming, and the
dependencies need swapping (Terp → R3).

The architecture mirrors WebSocket.Rx: a long-running async operation (there: WebSocket
connection; here: FFmpeg process) surfaced as R3 Observables with lifecycle events. The
key difference is that a WebSocket connection is long-lived and bidirectional, while an
FFmpeg process is short-lived and unidirectional (stdout/stderr out, cancellation in).

FunkArr's current FFmpeg usage (`FfmpegProcess.cs`) is the reference implementation:
remux with optional subtitles, progress parsing, error classification. The design
generalizes this into a reusable library without FunkArr-specific types.

## Goals / Non-Goals

**Goals:**
- Reactive FFmpeg process execution via R3 Observables
- Fluent, readable pipeline construction that mirrors FFmpeg CLI argument order
- Immutable pipeline records as the canonical model (builder is sugar)
- Zero-allocation progress parsing where feasible (hot path)
- Testable without a real FFmpeg binary (pure-function parsers, injectable process runner)
- NuGet-publishable standalone library

**Non-Goals:**
- No ffprobe / media info queries
- No filter graph DSL (consumers use `.Arg("-vf ...")`)
- No encoding presets or codec knowledge beyond `Codec.Copy`
- No DI integration package (later)
- No cross-platform binary bundling (consumer provides FFmpeg)

## Decisions

**D1: R3 over System.Reactive.** Same choice as WebSocket.Rx. R3 (Cysharp) is a modern
reimplementation with better allocation profile and `Observable<T>` as a concrete type
rather than an interface. Alternative: System.Reactive. Rejected: WebSocket.Rx already
validates R3 in the same ecosystem, and R3's `Subject<T>` is more ergonomic for the
process-pipe-to-observable bridge.

**D2: Single `Observable<FfmpegEvent>` with typed projections.** The core event stream is
`Observable<FfmpegEvent>` (sealed record hierarchy). Convenience properties
(`.Progress`, `.Log`) are `Events.OfType<T>()` — no extra state, just filtered views.
Alternative: Separate `Subject<T>` per event type (WebSocket.Rx pattern). Rejected for
FFmpeg because the event flow is strictly sequential (Started → Progress* → Completed|Failed),
so a single stream with discriminated events is more natural and composable.

**D3: Immutable records as pipeline model.** `FfmpegPipeline`, `FfmpegInput`, `FfmpegOutput`,
`StreamMapping` are all immutable records. The fluent builder constructs these internally
and `Build()` freezes them into a pipeline. Tests can construct records directly without
the builder. Alternative: mutable builder state exposed as the model. Rejected: immutable
records are easier to reason about and test.

**D4: One session = one process.** `FfmpegSession` is created via `pipeline.Build()`, runs
once via `RunAsync()`, and is then spent. To run again, build a new session from the same
(immutable) pipeline. Alternative: reusable sessions with state reset. Rejected: adds
complexity (what happens on double-run?), and the pipeline is already reusable.

**D5: Own process wrapper, no CliWrap.** A thin internal `ProcessRunner` wraps
`System.Diagnostics.Process`, reads stdout/stderr line-by-line, and pushes into R3
`Subject<T>`. Alternative: CliWrap. Rejected: CliWrap adds a dependency and its pipe
model would need adapting to R3 Subjects — the bridge code is as complex as the raw
Process wrapper. The wrapper is ~80 lines.

**D6: Error classification in the lib.** `ErrorClassifier` maps known FFmpeg stderr
patterns to `FailureKind.Transient` (HTTP 503, connection reset, timeout) or
`FailureKind.Permanent` (404, invalid data, file not found). Unknown errors default to
`Permanent`. Consumer can inspect the raw error string for app-specific classification.
Alternative: consumer-only classification. Rejected: the FFmpeg error patterns are
universal, every consumer would reimplement the same thing.

**D7: FFmpeg binary discovery.** Default: `ffmpeg` on PATH. Override:
`Ffmpeg.At("/path/to/ffmpeg").Input(...)` sets the binary for that pipeline. Future:
Options pattern for DI scenarios. The binary path is stored on the pipeline record.

**D8: Progress via `-progress pipe:1`.** FFmpeg writes key=value progress blocks to
stdout when `-progress pipe:1` is passed. The parser is a pure function
(`ProgressParser.TryParse`) extracted from FunkArr's existing `ParseProgressLine`.
The builder method `.WithProgressReporting()` adds the argument automatically.

## Risks / Trade-offs

- [R3 is less widely adopted than System.Reactive] → WebSocket.Rx already validates
  it in production. R3 is actively maintained by Cysharp with strong .NET focus.
- [No ffprobe means callers can't inspect media before processing] → Intentional
  scope limit. A future `FFmpeg.Rx.Probe` package could add this.
- [Own process wrapper means owning edge cases (zombie processes, pipe deadlocks)] →
  The wrapper is simple (line-by-line reads, no concurrent stdout+stderr — stderr is
  collected as a string on process exit). FunkArr has run the equivalent code in
  production.
- [Error classification patterns may not cover all FFmpeg versions] → The classifier
  is conservative (unknown = Permanent). Consumers can inspect the raw error string.
  Patterns can be extended over time.
