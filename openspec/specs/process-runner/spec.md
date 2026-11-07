## ADDED Requirements

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
