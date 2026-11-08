# pipe-io Specification

## Purpose
Pipe I/O interfaces, stream adapters, builder integration, platform-specific pipe implementation, and progress routing for pipe modes.
## Requirements
### Requirement: IPipeSource interface
The system SHALL define `IPipeSource` with a `Task WriteAsync(Stream pipeStream, CancellationToken ct)` method and a `StreamFormat` property (format string for ffmpeg, e.g. "wav", "rawvideo").

#### Scenario: Implement custom pipe source
- **WHEN** user implements `IPipeSource` and passes it to `PipeInput`
- **THEN** the system calls `WriteAsync` with the pipe's write stream when ffmpeg is ready

### Requirement: IPipeSink interface
The system SHALL define `IPipeSink` with a `Task ReadAsync(Stream pipeStream, CancellationToken ct)` method.

#### Scenario: Implement custom pipe sink
- **WHEN** user implements `IPipeSink` and passes it to `PipeOutput`
- **THEN** the system calls `ReadAsync` with the pipe's read stream when ffmpeg starts writing

### Requirement: StreamPipeSource
The system SHALL provide `StreamPipeSource(Stream source, string format)` implementing `IPipeSource`.

#### Scenario: Pipe from .NET stream
- **WHEN** user creates `new StreamPipeSource(fileStream, "wav")`
- **THEN** the stream's content is piped to ffmpeg as input

### Requirement: StreamPipeSink
The system SHALL provide `StreamPipeSink(Stream destination)` implementing `IPipeSink`.

#### Scenario: Pipe to .NET stream
- **WHEN** user creates `new StreamPipeSink(memoryStream)`
- **THEN** ffmpeg output is written to the provided stream

### Requirement: Pipe input on builder
`PipelineBuilder` SHALL provide `.Input(IPipeSource source, Action<InputOptions>? configure)`.

#### Scenario: Build pipeline with pipe input
- **WHEN** user calls `Ffmpeg.Input(new StreamPipeSource(stream, "wav"))`
- **THEN** the pipeline creates a named pipe (Windows) or anonymous pipe (Unix) and feeds `-f wav -i pipe:` or the pipe path to ffmpeg

### Requirement: Pipe output on builder
`PipelineBuilder` SHALL provide `.OutputPipe(IPipeSink sink, Action<OutputOptions> configure)`.

#### Scenario: Build pipeline with pipe output
- **WHEN** user calls `.OutputPipe(new StreamPipeSink(outStream), o => o.ForceFormat("wav"))`
- **THEN** the pipeline creates a pipe for output and ffmpeg writes to it

### Requirement: Platform-specific pipe implementation
On Windows, the system SHALL use named pipes. On Unix/macOS, the system SHALL use Unix domain sockets or anonymous pipes.

#### Scenario: Windows pipe
- **WHEN** running on Windows
- **THEN** pipe I/O uses `NamedPipeServerStream` / `NamedPipeClientStream`

#### Scenario: Unix pipe
- **WHEN** running on Linux or macOS
- **THEN** pipe I/O uses anonymous pipes or Unix domain sockets

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

