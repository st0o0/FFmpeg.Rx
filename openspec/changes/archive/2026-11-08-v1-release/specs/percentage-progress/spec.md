## ADDED Requirements

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
