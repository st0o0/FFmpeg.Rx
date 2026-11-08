## ADDED Requirements

### Requirement: PipelineBuilder produces PipelineSpec
`PipelineBuilder` SHALL collect `InputSpec` records (not `FfmpegInput`) and produce `PipelineSpec` (not `FfmpegPipeline`). Output methods SHALL use `OutputBuilder` to create `OutputSpec`.

#### Scenario: Builder produces typed spec
- **WHEN** `Ffmpeg.Input("v.mp4").Output("out.mp4", o => o.Stream(...))` is called
- **THEN** the session is backed by a `PipelineSpec` with typed records throughout

### Requirement: Ffmpeg static entry uses new InputSpec
`Ffmpeg.Input(string)` SHALL create `FileInputSpec`. `Ffmpeg.Input(Uri)` SHALL create `UrlInputSpec`. `Ffmpeg.Input(Stream, string)` SHALL create `PipeInputSpec`.

#### Scenario: File input
- **WHEN** `Ffmpeg.Input("video.mp4")` is called
- **THEN** a `FileInputSpec` is added to the builder

### Requirement: Pipe input via Stream
`PipelineBuilder.Input(Stream, string format)` SHALL create a `PipeInputSpec` that carries the stream reference directly. No pipe name generation, no Named Pipe paths.

#### Scenario: Stream input
- **WHEN** `Ffmpeg.Input(myStream, "rawvideo")` is called
- **THEN** a `PipeInputSpec` with the stream is created

### Requirement: TeeOutput migrated to new model
`TeeOutput` and `TeeTarget` SHALL be moved to the new model (or kept as-is since `OutputSpec` already references `TeeOutput`). The tee serialization in `ArgumentSerializer` stays.

#### Scenario: Tee output
- **WHEN** a tee output is configured
- **THEN** it is part of `OutputSpec.Tee` and serialized correctly
