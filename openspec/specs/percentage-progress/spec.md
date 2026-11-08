# percentage-progress Specification

## Purpose
TBD - created by archiving change ffmpeg-rx-v1-roadmap. Update Purpose after archive.
## Requirements
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

