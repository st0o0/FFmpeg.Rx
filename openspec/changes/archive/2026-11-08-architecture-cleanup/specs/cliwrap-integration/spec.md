## ADDED Requirements

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
- **WHEN** a `PipeInputConfig` carries a Stream
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
