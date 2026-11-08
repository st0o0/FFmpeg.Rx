## ADDED Requirements

### Requirement: Single model hierarchy
The library SHALL use only the typed `*Spec` record hierarchy (`PipelineSpec`, `OutputSpec`, `InputSpec`, `StreamSpec`, `CodecSpec`). No parallel string-based model types.

#### Scenario: No old types
- **WHEN** the Models namespace is inspected
- **THEN** only `*Spec` records, `OutputDestination` hierarchy, `TeeOutput`, `FfmpegResult`, and `FilterSpec` exist

### Requirement: Single session construction path
`FfmpegSession` SHALL have exactly one factory: `Create(PipelineSpec)`.

#### Scenario: One factory method
- **WHEN** `FfmpegSession.Create` is called
- **THEN** it only accepts `PipelineSpec`

### Requirement: No IO abstraction layer
Pipe I/O SHALL use CliWrap's `PipeSource`/`PipeTarget` directly. No library-owned pipe interfaces.

#### Scenario: No IO directory
- **WHEN** the project structure is inspected
- **THEN** there is no `IO/` directory
