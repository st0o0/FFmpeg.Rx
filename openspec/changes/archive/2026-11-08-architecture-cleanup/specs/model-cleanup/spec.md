## ADDED Requirements

### Requirement: Single model hierarchy
The library SHALL use only the typed `*Spec` record hierarchy (`PipelineConfig`, `OutputConfig`, `InputConfig`, `StreamConfig`, `CodecConfig`). No parallel string-based model types.

#### Scenario: No old types
- **WHEN** the Models namespace is inspected
- **THEN** only `*Spec` records, `OutputDestination` hierarchy, `TeeOutput`, `FfmpegResult`, and `FilterConfig` exist

### Requirement: Single session construction path
`FfmpegSession` SHALL have exactly one factory: `Create(PipelineConfig)`.

#### Scenario: One factory method
- **WHEN** `FfmpegSession.Create` is called
- **THEN** it only accepts `PipelineConfig`

### Requirement: No IO abstraction layer
Pipe I/O SHALL use CliWrap's `PipeSource`/`PipeTarget` directly. No library-owned pipe interfaces.

#### Scenario: No IO directory
- **WHEN** the project structure is inspected
- **THEN** there is no `IO/` directory
