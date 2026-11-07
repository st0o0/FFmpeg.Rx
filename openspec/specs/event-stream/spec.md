## ADDED Requirements

### Requirement: FfmpegSession exposes a single Observable event stream
`FfmpegSession` SHALL expose an `Observable<FfmpegEvent> Events` property that emits all lifecycle events for one FFmpeg process run. The stream SHALL complete after emitting exactly one terminal event (`CompletedEvent` or `FailedEvent`).

#### Scenario: Successful process run
- **WHEN** an FFmpeg process exits with code 0
- **THEN** `Events` SHALL emit `StartedEvent`, zero or more `ProgressEvent`/`LogEvent`, then exactly one `CompletedEvent`, then complete

#### Scenario: Failed process run
- **WHEN** an FFmpeg process exits with a non-zero code
- **THEN** `Events` SHALL emit `StartedEvent`, zero or more `ProgressEvent`/`LogEvent`, then exactly one `FailedEvent`, then complete

#### Scenario: Cancelled process run
- **WHEN** the CancellationToken is cancelled during `RunAsync`
- **THEN** `Events` SHALL emit a `FailedEvent` with exit code -1 and error "Cancelled", then complete

### Requirement: Typed convenience projections
`FfmpegSession` SHALL expose typed Observable properties that are filtered views of `Events`: `Progress` (ProgressEvent), `Log` (LogEvent). These SHALL NOT maintain separate state — they are `Events.OfType<T>()` projections.

#### Scenario: Progress projection
- **WHEN** a subscriber observes `session.Progress`
- **THEN** it SHALL receive only `ProgressEvent` instances from the `Events` stream

#### Scenario: Log projection
- **WHEN** a subscriber observes `session.Log`
- **THEN** it SHALL receive only `LogEvent` instances from the `Events` stream

### Requirement: Event hierarchy uses sealed records
All event types SHALL be sealed records inheriting from `abstract record FfmpegEvent`. The hierarchy SHALL be: `StartedEvent(int ProcessId, string Arguments)`, `ProgressEvent(long BytesWritten, long TimeUs, double Speed)`, `LogEvent(string Line, LogLevel Level)`, `CompletedEvent(int ExitCode, TimeSpan Duration)`, `FailedEvent(int ExitCode, string Error, FailureKind Kind)`.

#### Scenario: Pattern matching on events
- **WHEN** a consumer pattern-matches on `FfmpegEvent`
- **THEN** the compiler SHALL enforce exhaustiveness across all five event types

### Requirement: RunAsync returns FfmpegResult
`FfmpegSession.RunAsync(CancellationToken)` SHALL return `Task<FfmpegResult>` where `FfmpegResult` is a record containing `bool Success`, `int ExitCode`, `string? Error`, `TimeSpan Duration`, and `FailureKind? FailureKind`. The task SHALL complete when the FFmpeg process exits.

#### Scenario: Subscribing before RunAsync
- **WHEN** a subscriber attaches to `Events` before calling `RunAsync`
- **THEN** it SHALL receive all events including `StartedEvent`

#### Scenario: RunAsync without subscribers
- **WHEN** `RunAsync` is called without any `Events` subscribers
- **THEN** it SHALL still execute the process and return `FfmpegResult`

### Requirement: Session is single-use
`FfmpegSession` SHALL execute exactly one FFmpeg process. Calling `RunAsync` a second time SHALL throw `InvalidOperationException`.

#### Scenario: Double RunAsync
- **WHEN** `RunAsync` is called on a session that has already been run
- **THEN** it SHALL throw `InvalidOperationException`
