# scoped-builders Specification

## Purpose
Scoped fluent builders that produce typed pipeline records — OutputBuilder, StreamBuilder, encoding builders, and pipeline entry points.
## Requirements
### Requirement: OutputBuilder scoped to output concerns
The system SHALL provide an `OutputBuilder` with methods: `.Stream()`, `.VideoFilter()`, `.AudioFilter()`, `.FilterComplex()`, `.Container()`, `.TimeRange()`, `.Metadata()`, `.FastStart()`, `.MovFlags()`, `.NoOverwrite()`, `.MapLabel()`, `.CustomArg()`. It SHALL produce an `OutputConfig` record.

#### Scenario: Build typed output
- **WHEN** user configures an output via `OutputBuilder`
- **THEN** the resulting `OutputConfig` has typed fields for each concern, not a string list

### Requirement: StreamBuilder with Copy/Encode fork
The system SHALL provide a `StreamBuilder` with `.Copy()` (terminal — no further encoding configuration) and `.Encode(VideoCodec, Action<VideoEncodingBuilder>?)`, `.Encode(AudioCodec, Action<AudioEncodingBuilder>?)`, `.Encode(SubtitleCodec)`. It SHALL also have `.Metadata(key, value)` and `.Disposition(flags)`.

#### Scenario: Copy is terminal
- **WHEN** user calls `s.Copy()` on a StreamBuilder
- **THEN** the StreamConfig gets a `StreamCopy` CodecConfig and no encoding builder is offered

#### Scenario: Encode opens encoding builder
- **WHEN** user calls `s.Encode(VideoCodec.H264, e => e.Quality(Quality.Crf(23)))`
- **THEN** the StreamConfig gets a `VideoEncode` with the configured options

### Requirement: VideoEncodingBuilder scoped to video concerns
`VideoEncodingBuilder` SHALL expose: `.Quality(QualityConfig)`, `.Preset(Preset)`, `.Profile(VideoProfile)`, `.Tune(Tune)`, `.PixelFormat(PixelFormat)`, `.Framerate(Framerate)`. It SHALL NOT expose audio-specific options.

#### Scenario: Video builder has Preset
- **WHEN** user has a VideoEncodingBuilder instance
- **THEN** `.Preset()` is available and `.SampleRate()` is not

### Requirement: AudioEncodingBuilder scoped to audio concerns
`AudioEncodingBuilder` SHALL expose: `.Bitrate(Bitrate)`, `.SampleRate(int)`, `.Channels(int)`. It SHALL NOT expose video-specific options like Preset or CRF.

#### Scenario: Audio builder has Bitrate
- **WHEN** user has an AudioEncodingBuilder instance
- **THEN** `.Bitrate()` is available and `.Preset()` is not

### Requirement: TimeRangeBuilder
`TimeRangeBuilder` SHALL expose `.Seek(TimeSpan)`, `.Duration(TimeSpan)`, `.StopTime(TimeSpan)` and produce a `TimeRange` record.

#### Scenario: Build time range
- **WHEN** user calls `.TimeRange(t => t.Seek(TimeSpan.FromSeconds(30)).Duration(TimeSpan.FromSeconds(10)))`
- **THEN** the OutputConfig.TimeRange has Seek=30s and Duration=10s as typed TimeSpans

### Requirement: MetadataBuilder
`MetadataBuilder` SHALL expose `.Global(key, value)`, `.StripAll()`, `.CopyFrom(inputIndex)` and produce a `MetadataConfig` record.

#### Scenario: Add global metadata
- **WHEN** user calls `.Metadata(m => m.Global("title", "My Video"))`
- **THEN** the OutputConfig.Metadata.GlobalEntries contains the key-value pair

### Requirement: PipelineBuilder produces typed PipelineConfig
`PipelineBuilder` SHALL collect inputs and produce `OutputConfig` records via `OutputBuilder`. The final `.Output()` call SHALL create an `FfmpegSession` from a fully typed `PipelineConfig`.

#### Scenario: End-to-end pipeline construction
- **WHEN** user calls `Ffmpeg.Input("video.mp4").Output("out.mp4", o => o.Stream(...))`
- **THEN** an `FfmpegSession` is created backed by a `PipelineConfig` with typed records throughout

### Requirement: Escape hatch on every builder
Each builder SHALL provide a `.CustomArg(string)` method for raw FFmpeg arguments not covered by the typed API. These SHALL be collected into the `CustomArgs` field of the corresponding record.

#### Scenario: Custom argument
- **WHEN** user calls `.CustomArg("-bsf:a aac_adtstoasc")` on an OutputBuilder
- **THEN** the OutputConfig.CustomArgs contains the raw string

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

