# bitstream-filters Specification

## Purpose
TBD - created by archiving change input-options-and-bsf. Update Purpose after archive.
## Requirements
### Requirement: BitstreamFilter on StreamConfig
`StreamConfig` SHALL include an optional `BitstreamFilter` property (`string?`, default `null`). When set, it specifies a bitstream filter to apply to that stream's output.

#### Scenario: Stream copy with BSF
- **WHEN** `new StreamConfig(0, 0, StreamType.Audio, new StreamCopy(), BitstreamFilter: "aac_adtstoasc")` is constructed
- **THEN** the record SHALL have `BitstreamFilter == "aac_adtstoasc"`

### Requirement: StreamBuilder.BitstreamFilter fluent method
`StreamBuilder` SHALL provide a `.BitstreamFilter(string filter)` method that sets the BSF on the built `StreamConfig`.

#### Scenario: BSF via builder
- **WHEN** `o.Stream(0, 0, StreamType.Audio, s => s.Copy().BitstreamFilter("aac_adtstoasc"))` is configured
- **THEN** the built `StreamConfig` SHALL have `BitstreamFilter == "aac_adtstoasc"`

### Requirement: BSF serialization per stream type
The `ArgumentSerializer` SHALL serialize `StreamConfig.BitstreamFilter` as `-bsf:<type> <filter>` where `<type>` is the stream type character (`v` for video, `a` for audio, `s` for subtitle).

#### Scenario: Audio BSF serialized
- **GIVEN** a stream with `Type = StreamType.Audio` and `BitstreamFilter = "aac_adtstoasc"`
- **WHEN** the pipeline is serialized
- **THEN** the output SHALL contain `-bsf:a aac_adtstoasc`

#### Scenario: Video BSF serialized
- **GIVEN** a stream with `Type = StreamType.Video` and `BitstreamFilter = "h264_mp4toannexb"`
- **WHEN** the pipeline is serialized
- **THEN** the output SHALL contain `-bsf:v h264_mp4toannexb`

#### Scenario: No BSF when null
- **GIVEN** a stream with `BitstreamFilter = null`
- **WHEN** the pipeline is serialized
- **THEN** no `-bsf:` argument SHALL appear for that stream

