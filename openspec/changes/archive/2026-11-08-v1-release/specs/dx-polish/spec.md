## ADDED Requirements

### Requirement: C#14 extension blocks
R3 composability operators SHALL be defined using C#14 `extension()` blocks on `IFfmpegSession`.

#### Scenario: Extension method available
- **WHEN** a consumer has an `IFfmpegSession`
- **THEN** `.PercentageProgress(duration)` is available as an extension method

### Requirement: TeeBuilder aligned with OutputConfig
`TeeBuilder` SHALL produce records compatible with `OutputConfig.Tee`. No separate `TeeTargetOptions` class — use `TeeTarget` record directly.

#### Scenario: Clean tee API
- **WHEN** a tee output is configured via PipelineBuilder
- **THEN** the resulting OutputConfig.Tee contains TeeTarget records

### Requirement: RELEASE_NOTES.md
The repository SHALL contain `RELEASE_NOTES.md` documenting changes for each version.

#### Scenario: Version history
- **WHEN** a consumer checks the changelog
- **THEN** RELEASE_NOTES.md lists changes grouped by version
