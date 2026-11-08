# argument-serializer Specification

## Purpose
TBD - created by archiving change api-redesign. Update Purpose after archive.
## Requirements
### Requirement: ArgumentSerializer replaces ArgumentBuilder
The system SHALL rename `ArgumentBuilder` to `ArgumentSerializer` and rewrite it to accept typed `PipelineConfig` records instead of string-carrying models.

#### Scenario: Serialize typed pipeline
- **WHEN** `ArgumentSerializer.Serialize(pipelineSpec)` is called with a fully typed PipelineConfig
- **THEN** it returns the complete FFmpeg CLI argument string

### Requirement: Pattern matching on CodecConfig
The serializer SHALL use pattern matching to serialize `CodecConfig` subtypes: `StreamCopy` → `-c:{type} copy`, `VideoEncode` → `-c:v {codec}` plus encoding options, `AudioEncode` → `-c:a {codec}` plus encoding options, `SubtitleEncode` → `-c:s {codec}`.

#### Scenario: Serialize video encode
- **WHEN** a StreamConfig has `VideoEncode(VideoCodec.H264, options with Crf=23, Preset=Slow)`
- **THEN** the serializer emits `-c:v libx264 -crf 23 -preset slow`

#### Scenario: Serialize stream copy
- **WHEN** a StreamConfig has `StreamCopy`
- **THEN** the serializer emits `-c:v copy` (or `-c:a copy` depending on StreamType)

### Requirement: Serialize value objects
The serializer SHALL call internal serialization methods on value objects: `Bitrate.ToFfmpegString()`, `Framerate.ToFfmpegString()`, and use the `Name` property on struct types.

#### Scenario: Serialize bitrate
- **WHEN** an AudioEncode has Bitrate of `Bitrate.Kbps(192)`
- **THEN** the serializer emits `-b:a 192k`

### Requirement: Serialize QualityConfig variants
The serializer SHALL pattern match on QualityConfig: `ConstantQuality(crf)` → `-crf {crf}`, `ConstantBitrate(rate)` → `-b:v {rate.ToFfmpegString()}`.

#### Scenario: Serialize CRF
- **WHEN** encoding options have `Quality = Quality.Crf(23)`
- **THEN** the serializer emits `-crf 23`

### Requirement: Serialize TimeRange
The serializer SHALL emit `-ss`, `-t`, `-to` from TimeRange fields, formatting TimeSpan values consistently.

#### Scenario: Serialize seek and duration
- **WHEN** output TimeRange has Seek=30s and Duration=10s
- **THEN** the serializer emits `-ss 00:00:30.000 -t 10.000`

### Requirement: Serialize filters unchanged
The serializer SHALL continue to serialize filter records using their `Render()` method, joining with commas for simple chains and semicolons for complex graphs.

#### Scenario: Serialize video filter chain
- **WHEN** output has SimpleFilterConfig with [Scale(1920,1080), HFlip]
- **THEN** the serializer emits `-vf "scale=1920:1080,hflip"`

### Requirement: Serialize custom args last
Custom args from `OutputConfig.CustomArgs` SHALL be emitted after all typed arguments and before the output path.

#### Scenario: Custom args position
- **WHEN** output has typed codec options AND custom args
- **THEN** custom args appear after the typed arguments

