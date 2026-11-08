# process-execution Specification

## Purpose
Manages FFmpeg/ffprobe process lifecycle via CliWrap — starting, event bridging, cancellation, and disposal.
## Requirements
### Requirement: Process lifecycle management
The internal `ProcessRunner` SHALL start an FFmpeg process with the arguments from an `FfmpegPipeline`, read stdout line-by-line for progress parsing, collect stderr, and report the exit code. It SHALL emit `StartedEvent` with the process ID and full argument string immediately after the process spawns.

#### Scenario: Process starts successfully
- **WHEN** `ProcessRunner` starts an FFmpeg process
- **THEN** it SHALL emit `StartedEvent` with the OS process ID and the CLI arguments as a string

#### Scenario: FFmpeg binary not found
- **WHEN** the configured FFmpeg binary path does not exist and is not on PATH
- **THEN** `RunAsync` SHALL throw or emit `FailedEvent` with a descriptive error

### Requirement: stdout is parsed line-by-line during execution
The process runner SHALL read stdout line-by-line while the process is running and pass each line to the progress parser. Parsed progress blocks SHALL be emitted as `ProgressEvent` via the event Subject.

#### Scenario: Progress lines emitted during execution
- **WHEN** FFmpeg writes progress blocks to stdout (via `-progress pipe:1`)
- **THEN** `ProgressEvent` instances SHALL be emitted in real-time, not buffered until exit

### Requirement: stderr is collected and classified on exit
The process runner SHALL collect stderr output. On non-zero exit, it SHALL extract the most relevant error line and classify it via `ErrorClassifier`. The result SHALL be emitted as `FailedEvent`.

#### Scenario: Non-zero exit with stderr
- **WHEN** the FFmpeg process exits with code 1 and stderr contains "HTTP error 403 Forbidden"
- **THEN** a `FailedEvent` SHALL be emitted with the error message and `FailureKind.Permanent`

### Requirement: Cancellation kills the process
When the `CancellationToken` passed to `RunAsync` is cancelled, the process runner SHALL kill the FFmpeg process. The event stream SHALL emit `FailedEvent(ExitCode: -1, Error: "Cancelled")` and complete.

#### Scenario: Cancellation during download
- **WHEN** the CancellationToken is cancelled while FFmpeg is running
- **THEN** the FFmpeg process SHALL be killed
- **AND** `FailedEvent` with exit code -1 SHALL be emitted

### Requirement: Session implements IAsyncDisposable
`FfmpegSession` SHALL implement `IAsyncDisposable`. Disposing a session that is still running SHALL cancel and kill the process.

#### Scenario: Dispose during execution
- **WHEN** `DisposeAsync` is called on a running session
- **THEN** the FFmpeg process SHALL be killed and resources released

### Requirement: CliWrap as process abstraction
The library SHALL use CliWrap for all external process execution (ffmpeg and ffprobe). No direct `System.Diagnostics.Process` usage.

#### Scenario: ffmpeg execution
- **WHEN** `FfmpegSession.RunAsync()` is called
- **THEN** it executes via `Cli.Wrap(binary).WithArguments(args)`, not `Process.Start`

### Requirement: FfmpegSession as runner
`FfmpegSession.RunAsync()` SHALL directly build and execute the CliWrap command. No separate `ProcessRunner` class.

#### Scenario: No ProcessRunner
- **WHEN** the Internal namespace is inspected
- **THEN** there is no `ProcessRunner` class

### Requirement: Real-time event bridging
`FfmpegSession` SHALL use CliWrap's `ListenAsync()` to receive stdout/stderr events and bridge them to R3 `Subject<FfmpegEvent>` in real-time.

#### Scenario: Progress events during transcode
- **WHEN** ffmpeg emits `-progress pipe:1` output
- **THEN** `ProgressParser` parses each line and emits `ProgressEvent` via the R3 Subject

### Requirement: Pipe I/O via CliWrap
Input piping SHALL use `PipeSource.FromStream` with `-i pipe:0`. Output piping SHALL use `PipeTarget.ToStream` with `pipe:1`. No Named Pipe servers.

#### Scenario: Stream input
- **WHEN** a `PipeInputSpec` carries a Stream
- **THEN** CliWrap reads from the stream via stdin, cross-platform

### Requirement: Graceful cancellation
Without stdin piping, cancellation SHALL write `q` to stdin. With stdin piping, cancellation SHALL use CliWrap's graceful token (Ctrl+C/SIGINT). Forceful kill is the fallback.

#### Scenario: Cancel without pipe
- **WHEN** cancellation is requested during a file-to-file transcode
- **THEN** `q` is written to stdin, ffmpeg exits gracefully

### Requirement: ProbeRunner uses CliWrap
`ProbeRunner` SHALL use `Cli.Wrap(ffprobe).ExecuteBufferedAsync()` instead of manual Process handling.

#### Scenario: Probe execution
- **WHEN** `Ffprobe.AnalyseAsync` is called
- **THEN** ffprobe runs via CliWrap and the JSON output is captured via `ExecuteBufferedAsync`
