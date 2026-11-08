# pipeline-model Specification

## Purpose
Immutable record hierarchy representing a complete FFmpeg invocation — PipelineConfig, InputConfig, OutputConfig, StreamConfig, CodecConfig, and supporting types.
## Requirements
### Requirement: PipelineConfig as root record
The system SHALL represent a complete FFmpeg invocation as an immutable `PipelineConfig` record containing `Inputs` (IReadOnlyList<InputConfig>), `Outputs` (IReadOnlyList<OutputConfig>), `GlobalOptions`, and optional `BinaryPath`.

#### Scenario: Pipeline with single input and output
- **WHEN** a pipeline is constructed with one FileInput and one OutputConfig
- **THEN** `PipelineConfig.Inputs` contains one element and `PipelineConfig.Outputs` contains one element

### Requirement: InputConfig hierarchy
The system SHALL represent inputs as an abstract `InputConfig` record with concrete subtypes: `FileInput(string Path)`, `UrlInput(Uri Url)`, `PipeInput(IPipeSource Source, string PipeName)`. Each SHALL carry optional `InputOptions` for seek, HW accel, decoder, and custom arguments.

#### Scenario: File input with seek
- **WHEN** a FileInput is created with `InputOptions { Seek = TimeSpan.FromSeconds(30) }`
- **THEN** the InputConfig carries the seek value as a typed TimeSpan, not a string

### Requirement: OutputConfig as typed record
The system SHALL represent output as an immutable `OutputConfig` record containing: `Destination` (OutputDestination), `Streams` (IReadOnlyList<StreamConfig>), `Container` (ContainerFormat?), `Filters` (FilterConfig?), `Metadata` (MetadataConfig?), `TimeRange` (TimeRange?), `Overwrite` (bool), `FastStart` (bool), `MovFlags` (IReadOnlyList<string>?), `CustomArgs` (IReadOnlyList<string>?).

#### Scenario: Output with no string bags
- **WHEN** an OutputConfig is constructed with encoding options, filters, and metadata
- **THEN** each concern is a typed field — no `string[] CustomArguments` carries domain data

### Requirement: StreamConfig with disjoint CodecConfig
The system SHALL represent each output stream as a `StreamConfig` record containing `InputIndex`, `StreamIndex` (nullable for "all of type"), `Type` (StreamType), `Codec` (CodecConfig), optional `Metadata`, and optional `Disposition`.

#### Scenario: Video stream with encoding
- **WHEN** a StreamConfig has `Codec` of type `VideoEncode`
- **THEN** the VideoEncode carries a `VideoCodec` and `VideoEncodingOptions`

#### Scenario: Stream copy has no encoding options
- **WHEN** a StreamConfig has `Codec` of type `StreamCopy`
- **THEN** there is no way to access encoding options — the type does not have them

### Requirement: CodecConfig hierarchy
The system SHALL define `CodecConfig` as an abstract record with exactly four concrete subtypes: `StreamCopy`, `VideoEncode(VideoCodec Codec, VideoEncodingOptions Options)`, `AudioEncode(AudioCodec Codec, AudioEncodingOptions Options)`, `SubtitleEncode(SubtitleCodec Codec)`.

#### Scenario: Pattern match on codec
- **WHEN** code pattern-matches on a CodecConfig value
- **THEN** the four cases are exhaustive and each carries type-specific data

### Requirement: VideoEncodingOptions and AudioEncodingOptions are separate types
`VideoEncodingOptions` SHALL contain: `Quality` (QualityConfig?), `Preset` (Preset?), `Profile` (VideoProfile?), `Tune` (Tune?), `PixelFormat` (PixelFormat?), `Framerate` (Framerate?). `AudioEncodingOptions` SHALL contain: `Bitrate` (Bitrate?), `SampleRate` (int?), `Channels` (int?).

#### Scenario: Video options have Preset, audio does not
- **WHEN** a VideoEncodingOptions is inspected
- **THEN** it has a Preset property. AudioEncodingOptions does not have Preset.

### Requirement: TimeRange record
The system SHALL define `TimeRange` as an immutable record with `Seek` (TimeSpan?), `Duration` (TimeSpan?), `StopTime` (TimeSpan?).

#### Scenario: Time range with seek and duration
- **WHEN** a TimeRange is created with Seek=30s and Duration=10s
- **THEN** both values are typed TimeSpan properties

### Requirement: MetadataConfig record
The system SHALL define `MetadataConfig` with `GlobalEntries` (IReadOnlyList<KeyValuePair<string,string>>?), `StripAll` (bool), `CopyFromInput` (int?).

#### Scenario: Strip all metadata
- **WHEN** MetadataConfig has StripAll=true
- **THEN** the serializer emits `-map_metadata -1`

### Requirement: FilterConfig as disjoint type
The system SHALL define `FilterConfig` as an abstract record with two subtypes: `SimpleFilterConfig(IReadOnlyList<IVideoFilter>?, IReadOnlyList<IAudioFilter>?)` and `ComplexFilterConfig(FilterGraph)`.

#### Scenario: Simple filters
- **WHEN** FilterConfig is SimpleFilterConfig with video filters
- **THEN** it carries the typed filter records from the existing filter system

### Requirement: No string[] CustomArguments on domain records
Domain records (OutputConfig, StreamConfig, EncodingOptions) SHALL NOT carry `string[]` for domain concerns. A `CustomArgs` escape hatch SHALL exist only on OutputConfig for truly custom flags.

#### Scenario: Encoding options are typed
- **WHEN** a VideoEncode is inspected
- **THEN** CRF is an int, Preset is a Preset struct, Profile is a VideoProfile struct — not strings

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

