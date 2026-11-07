## ADDED Requirements

### Requirement: IFfmpegSession interface
The system SHALL define `IFfmpegSession` as a public interface extending `IAsyncDisposable` with: `Observable<FfmpegEvent> Events`, `Observable<ProgressEvent> Progress`, `Observable<LogEvent> Log`, `Task<FfmpegResult> RunAsync(CancellationToken ct)`.

#### Scenario: Consumer programs against interface
- **WHEN** a consumer declares a dependency on `IFfmpegSession`
- **THEN** they can mock it for testing without requiring an ffmpeg binary

### Requirement: FfmpegSession implements IFfmpegSession
`FfmpegSession` SHALL implement `IFfmpegSession`. Its constructor SHALL remain internal. Creation SHALL happen via `PipelineBuilder.Output()` or `FfmpegSession.Create(PipelineSpec)`.

#### Scenario: Session creation
- **WHEN** user calls `Ffmpeg.Input(...).Output(...)`
- **THEN** the returned object implements `IFfmpegSession`

### Requirement: Static entry returns IFfmpegSession
`PipelineBuilder.Output()` and `MultiOutputBuilder.Build()` SHALL return `IFfmpegSession`, not `FfmpegSession`.

#### Scenario: Return type
- **WHEN** user calls `.Output("out.mp4", o => ...)`
- **THEN** the compile-time return type is `IFfmpegSession`
