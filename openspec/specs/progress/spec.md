# progress Specification

## Purpose
Parses FFmpeg progress output and calculates percentage completion.
## Requirements
### Requirement: Parse FFmpeg progress blocks from stdout
`ProgressParser` SHALL parse FFmpeg's `-progress pipe:1` output format: key=value lines terminated by a `progress=continue` or `progress=end` line. Each complete block SHALL produce a `ProgressEvent`.

#### Scenario: Complete progress block
- **WHEN** stdout contains `total_size=5000000\nout_time_us=3000000\nspeed=1.5x\nprogress=continue\n`
- **THEN** a `ProgressEvent(BytesWritten: 5000000, TimeUs: 3000000, Speed: 1.5)` SHALL be produced

#### Scenario: Partial block (no progress= line yet)
- **WHEN** stdout contains `total_size=5000000\nout_time_us=3000000\n` without a `progress=` terminator
- **THEN** no `ProgressEvent` SHALL be produced until the block is complete

#### Scenario: Speed value "N/A"
- **WHEN** the `speed` field is `N/A`
- **THEN** `Speed` SHALL be `0.0`

#### Scenario: Missing fields
- **WHEN** a progress block is missing `total_size` or `out_time_us`
- **THEN** the missing values SHALL default to `0`

### Requirement: Parser is stateful per process and pure per block
`ProgressParser` SHALL accumulate key=value pairs across lines until a `progress=` line completes the block. After emitting, the accumulator SHALL be cleared for the next block. The block-to-event conversion SHALL be a pure function.

#### Scenario: Multiple consecutive blocks
- **WHEN** two complete progress blocks are received sequentially
- **THEN** two separate `ProgressEvent` instances SHALL be produced with independent values

### Requirement: Parser handles malformed input gracefully
Lines without `=` or with empty keys SHALL be ignored. The parser SHALL never throw.

#### Scenario: Malformed line
- **WHEN** stdout contains a line without `=` (e.g. a warning message mixed into stdout)
- **THEN** the line SHALL be ignored and parsing SHALL continue

### Requirement: Percentage progress extension
The system SHALL provide an extension method `ProgressEvent.AsPercentage(TimeSpan totalDuration)` that returns a `double` between 0.0 and 100.0.

#### Scenario: Calculate percentage from progress
- **WHEN** a `ProgressEvent` has `TimeUs = 30_000_000` (30 seconds) and total duration is 60 seconds
- **THEN** `AsPercentage(TimeSpan.FromSeconds(60))` returns `50.0`

#### Scenario: Zero duration
- **WHEN** total duration is `TimeSpan.Zero`
- **THEN** `AsPercentage` returns `0.0` (no division by zero)

### Requirement: Percentage progress Observable operator
The system SHALL provide an R3 extension `Observable<ProgressEvent>.WithPercentage(TimeSpan totalDuration)` that projects each `ProgressEvent` to a `PercentageProgressEvent(double Percent, ProgressEvent Source)`.

#### Scenario: Stream percentage events
- **WHEN** user subscribes to `session.Progress.WithPercentage(info.Duration)`
- **THEN** each emitted `PercentageProgressEvent` contains a `Percent` value and the original `ProgressEvent`

### Requirement: Graceful cancellation via q
When a `CancellationToken` is triggered, the system SHALL write `q` to ffmpeg's stdin and wait up to 5 seconds for graceful exit. If ffmpeg has not exited after 5 seconds, the system SHALL kill the process.

#### Scenario: Graceful cancellation
- **WHEN** cancellation is requested while ffmpeg is running
- **THEN** the system writes `q` to stdin, waits for ffmpeg to finalize output, and returns a result with `Success = false` and `Error = "Cancelled"`

#### Scenario: Graceful cancellation timeout
- **WHEN** cancellation is requested and ffmpeg does not exit within 5 seconds after receiving `q`
- **THEN** the system kills the process tree

#### Scenario: Cancellation after process exit
- **WHEN** cancellation is requested but ffmpeg has already exited
- **THEN** the system does not attempt to write to stdin or kill the process

### Requirement: WithProgressReporting auto-wires progress
`PipelineBuilder.WithProgressReporting(TimeSpan? knownDuration = null)` SHALL add `-progress pipe:1` to global options and store the known duration for percentage calculation.

#### Scenario: Known duration
- **WHEN** `.WithProgressReporting(TimeSpan.FromMinutes(5))` is called
- **THEN** ProgressEvents are emitted AND PercentageProgressEvents with correct percentage

### Requirement: Auto-probe duration if not provided
If `knownDuration` is null and the pipeline has a single file input, `FfmpegSession` SHALL probe the input duration via `Ffprobe.AnalyseAsync` before starting the process.

#### Scenario: Auto-probe
- **WHEN** `.WithProgressReporting()` is called without duration on a file input
- **THEN** the input is probed and PercentageProgressEvents use the probed duration

### Requirement: PercentageProgressEvent emission
`FfmpegSession` SHALL emit `PercentageProgressEvent` alongside `ProgressEvent` when a total duration is known. Percentage is calculated as `timeUs / totalDurationUs * 100`.

#### Scenario: Percentage calculation
- **WHEN** a ProgressEvent with timeUs=2500000 is received and total duration is 5 seconds
- **THEN** a PercentageProgressEvent with Percent=50.0 is emitted
