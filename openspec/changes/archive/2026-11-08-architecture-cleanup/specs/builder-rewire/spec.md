## ADDED Requirements

### Requirement: PipelineBuilder produces PipelineConfig
`PipelineBuilder` SHALL collect `InputConfig` records (not `FfmpegInput`) and produce `PipelineConfig` (not `FfmpegPipeline`). Output methods SHALL use `OutputBuilder` to create `OutputConfig`.

#### Scenario: Builder produces typed spec
- **WHEN** `Ffmpeg.Input("v.mp4").Output("out.mp4", o => o.Stream(...))` is called
- **THEN** the session is backed by a `PipelineConfig` with typed records throughout

### Requirement: Ffmpeg static entry uses new InputConfig
`Ffmpeg.Input(string)` SHALL create `FileInputConfig`. `Ffmpeg.Input(Uri)` SHALL create `UrlInputConfig`. `Ffmpeg.Input(Stream, string)` SHALL create `PipeInputConfig`.

#### Scenario: File input
- **WHEN** `Ffmpeg.Input("video.mp4")` is called
- **THEN** a `FileInputConfig` is added to the builder

### Requirement: Pipe input via Stream
`PipelineBuilder.Input(Stream, string format)` SHALL create a `PipeInputConfig` that carries the stream reference directly. No pipe name generation, no Named Pipe paths.

#### Scenario: Stream input
- **WHEN** `Ffmpeg.Input(myStream, "rawvideo")` is called
- **THEN** a `PipeInputConfig` with the stream is created

### Requirement: TeeOutput migrated to new model
`TeeOutput` and `TeeTarget` SHALL be moved to the new model (or kept as-is since `OutputConfig` already references `TeeOutput`). The tee serialization in `ArgumentSerializer` stays.

#### Scenario: Tee output
- **WHEN** a tee output is configured
- **THEN** it is part of `OutputConfig.Tee` and serialized correctly
