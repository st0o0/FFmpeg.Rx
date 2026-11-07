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

