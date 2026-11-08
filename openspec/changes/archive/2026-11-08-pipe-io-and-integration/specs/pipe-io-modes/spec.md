## ADDED Requirements

### Requirement: Progress stderr fallback
When any output uses `PipeDestination`, the serializer SHALL emit `-progress pipe:2` instead of `-progress pipe:1` to route progress to stderr.

#### Scenario: Pipe output with progress
- **WHEN** `WithProgressReporting()` is used with `OutputPipe()`
- **THEN** the CLI arguments contain `-progress pipe:2` and progress events are still emitted

### Requirement: Stderr progress parsing
In pipe-output mode, `FfmpegSession` SHALL parse progress key=value lines from stderr alongside log lines. A line is progress if `ProgressParser.ParseLine` returns non-null, otherwise it is a log event.

#### Scenario: Mixed stderr
- **WHEN** ffmpeg writes both progress and log lines to stderr
- **THEN** progress lines produce ProgressEvents and log lines produce LogEvents

### Requirement: No silent failure on conflicting modes
The library SHALL handle all four mode combinations without throwing or producing corrupt output.

#### Scenario: Pipe input works
- **WHEN** a Stream is piped as input
- **THEN** ffmpeg reads from stdin and produces output

#### Scenario: Pipe output works
- **WHEN** output is piped to a Stream
- **THEN** ffmpeg writes to stdout and the stream receives data
