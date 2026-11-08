# pipeline-model Specification

## Purpose
Immutable record hierarchy representing a complete FFmpeg invocation — PipelineSpec, InputSpec, OutputSpec, StreamSpec, CodecSpec, and supporting types.
## Requirements
### Requirement: PipelineSpec as root record
The system SHALL represent a complete FFmpeg invocation as an immutable `PipelineSpec` record containing `Inputs` (IReadOnlyList<InputSpec>), `Outputs` (IReadOnlyList<OutputSpec>), `GlobalOptions`, and optional `BinaryPath`.

#### Scenario: Pipeline with single input and output
- **WHEN** a pipeline is constructed with one FileInput and one OutputSpec
- **THEN** `PipelineSpec.Inputs` contains one element and `PipelineSpec.Outputs` contains one element

### Requirement: InputSpec hierarchy
The system SHALL represent inputs as an abstract `InputSpec` record with concrete subtypes: `FileInput(string Path)`, `UrlInput(Uri Url)`, `PipeInput(IPipeSource Source, string PipeName)`. Each SHALL carry optional `InputOptions` for seek, HW accel, decoder, and custom arguments.

#### Scenario: File input with seek
- **WHEN** a FileInput is created with `InputOptions { Seek = TimeSpan.FromSeconds(30) }`
- **THEN** the InputSpec carries the seek value as a typed TimeSpan, not a string

### Requirement: OutputSpec as typed record
The system SHALL represent output as an immutable `OutputSpec` record containing: `Destination` (OutputDestination), `Streams` (IReadOnlyList<StreamSpec>), `Container` (ContainerFormat?), `Filters` (FilterSpec?), `Metadata` (MetadataSpec?), `TimeRange` (TimeRange?), `Overwrite` (bool), `FastStart` (bool), `MovFlags` (IReadOnlyList<string>?), `CustomArgs` (IReadOnlyList<string>?).

#### Scenario: Output with no string bags
- **WHEN** an OutputSpec is constructed with encoding options, filters, and metadata
- **THEN** each concern is a typed field — no `string[] CustomArguments` carries domain data

### Requirement: StreamSpec with disjoint CodecSpec
The system SHALL represent each output stream as a `StreamSpec` record containing `InputIndex`, `StreamIndex` (nullable for "all of type"), `Type` (StreamType), `Codec` (CodecSpec), optional `Metadata`, and optional `Disposition`.

#### Scenario: Video stream with encoding
- **WHEN** a StreamSpec has `Codec` of type `VideoEncode`
- **THEN** the VideoEncode carries a `VideoCodec` and `VideoEncodingOptions`

#### Scenario: Stream copy has no encoding options
- **WHEN** a StreamSpec has `Codec` of type `StreamCopy`
- **THEN** there is no way to access encoding options — the type does not have them

### Requirement: CodecSpec hierarchy
The system SHALL define `CodecSpec` as an abstract record with exactly four concrete subtypes: `StreamCopy`, `VideoEncode(VideoCodec Codec, VideoEncodingOptions Options)`, `AudioEncode(AudioCodec Codec, AudioEncodingOptions Options)`, `SubtitleEncode(SubtitleCodec Codec)`.

#### Scenario: Pattern match on codec
- **WHEN** code pattern-matches on a CodecSpec value
- **THEN** the four cases are exhaustive and each carries type-specific data

### Requirement: VideoEncodingOptions and AudioEncodingOptions are separate types
`VideoEncodingOptions` SHALL contain: `Quality` (QualitySpec?), `Preset` (Preset?), `Profile` (VideoProfile?), `Tune` (Tune?), `PixelFormat` (PixelFormat?), `Framerate` (Framerate?). `AudioEncodingOptions` SHALL contain: `Bitrate` (Bitrate?), `SampleRate` (int?), `Channels` (int?).

#### Scenario: Video options have Preset, audio does not
- **WHEN** a VideoEncodingOptions is inspected
- **THEN** it has a Preset property. AudioEncodingOptions does not have Preset.

### Requirement: TimeRange record
The system SHALL define `TimeRange` as an immutable record with `Seek` (TimeSpan?), `Duration` (TimeSpan?), `StopTime` (TimeSpan?).

#### Scenario: Time range with seek and duration
- **WHEN** a TimeRange is created with Seek=30s and Duration=10s
- **THEN** both values are typed TimeSpan properties

### Requirement: MetadataSpec record
The system SHALL define `MetadataSpec` with `GlobalEntries` (IReadOnlyList<KeyValuePair<string,string>>?), `StripAll` (bool), `CopyFromInput` (int?).

#### Scenario: Strip all metadata
- **WHEN** MetadataSpec has StripAll=true
- **THEN** the serializer emits `-map_metadata -1`

### Requirement: FilterSpec as disjoint type
The system SHALL define `FilterSpec` as an abstract record with two subtypes: `SimpleFilterSpec(IReadOnlyList<IVideoFilter>?, IReadOnlyList<IAudioFilter>?)` and `ComplexFilterSpec(FilterGraph)`.

#### Scenario: Simple filters
- **WHEN** FilterSpec is SimpleFilterSpec with video filters
- **THEN** it carries the typed filter records from the existing filter system

### Requirement: No string[] CustomArguments on domain records
Domain records (OutputSpec, StreamSpec, EncodingOptions) SHALL NOT carry `string[]` for domain concerns. A `CustomArgs` escape hatch SHALL exist only on OutputSpec for truly custom flags.

#### Scenario: Encoding options are typed
- **WHEN** a VideoEncode is inspected
- **THEN** CRF is an int, Preset is a Preset struct, Profile is a VideoProfile struct — not strings

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

