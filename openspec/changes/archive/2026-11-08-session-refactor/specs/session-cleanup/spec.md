## ADDED Requirements

### Requirement: Single execution path
FfmpegSession SHALL have one internal execution method, not two. The execution mode (EventStream vs PipeOutput) SHALL be determined from PipelineSpec and passed as configuration.

#### Scenario: File-to-file uses EventStream mode
- **WHEN** all outputs use FileDestination
- **THEN** the session uses ListenAsync with stdout progress

#### Scenario: Pipe output uses PipeOutput mode
- **WHEN** any output uses PipeDestination
- **THEN** the session uses ExecuteAsync with stderr progress

### Requirement: Pure progress emission
Progress and percentage calculation SHALL be a pure function `EmitProgress(ProgressEvent, TimeSpan?)` returning events. No inline duplication.

#### Scenario: Progress with duration
- **WHEN** EmitProgress is called with a ProgressEvent and 5s duration
- **THEN** it returns ProgressEvent + PercentageProgressEvent

### Requirement: Stderr line classification
A pure function SHALL classify stderr lines as either ProgressEvent (if parseable) or LogEvent. The `parseProgress` flag controls whether progress parsing is attempted.

#### Scenario: Progress line on stderr in pipe mode
- **WHEN** a progress key=value line arrives on stderr with parseProgress=true
- **THEN** it is classified as ProgressEvent, not LogEvent

### Requirement: R3 extension methods
C#14 `extension()` block SHALL provide composable R3 operators on `IFfmpegSession`.

#### Scenario: Consumer percentage progress
- **WHEN** consumer calls `session.PercentageProgress(duration)`
- **THEN** it returns an `Observable<double>` with percentage values
