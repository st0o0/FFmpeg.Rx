# value-objects Specification

## Purpose
TBD - created by archiving change api-redesign. Update Purpose after archive.
## Requirements
### Requirement: VideoCodec as readonly record struct
The system SHALL define `VideoCodec` as a `readonly record struct` with a `Name` property, static factory properties for well-known codecs (H264, Hevc, Vp9, Av1, H264Nvenc, HevcNvenc, H264Qsv, HevcQsv, Copy), and a `Custom(string name)` static factory.

#### Scenario: Type safety between video and audio
- **WHEN** a method parameter is typed as `VideoCodec`
- **THEN** passing an `AudioCodec` value SHALL produce a compile-time error

#### Scenario: Custom codec
- **WHEN** user calls `VideoCodec.Custom("libsvtav1")`
- **THEN** it creates a valid VideoCodec with Name "libsvtav1"

### Requirement: AudioCodec as readonly record struct
The system SHALL define `AudioCodec` as a `readonly record struct` with static factories for Aac, Opus, Mp3, Flac, Pcm16Le, Copy, and `Custom()`.

#### Scenario: Well-known audio codec
- **WHEN** user references `AudioCodec.Aac`
- **THEN** its Name is "aac"

### Requirement: SubtitleCodec as readonly record struct
The system SHALL define `SubtitleCodec` as a `readonly record struct` with static factories for MovText, Srt, Ass, WebVtt, Copy, and `Custom()`.

#### Scenario: Well-known subtitle codec
- **WHEN** user references `SubtitleCodec.MovText`
- **THEN** its Name is "mov_text"

### Requirement: Bitrate value object
The system SHALL define `Bitrate` as a `readonly record struct` with `BitsPerSecond` property, static factories `Kbps(int)`, `Mbps(int)`, `Mbps(double)`, and an internal `ToFfmpegString()` method.

#### Scenario: Create bitrate from kilobits
- **WHEN** user calls `Bitrate.Kbps(192)`
- **THEN** `BitsPerSecond` is 192000 and ffmpeg string is "192k"

#### Scenario: Create bitrate from megabits
- **WHEN** user calls `Bitrate.Mbps(4)`
- **THEN** `BitsPerSecond` is 4000000 and ffmpeg string is "4M"

### Requirement: Framerate value object
The system SHALL define `Framerate` as a `readonly record struct` with `Numerator` and `Denominator` properties, static factories `Fps(int)`, `Fps(double)`, `Rational(int, int)`, and well-known NTSC constants (Ntsc24, Ntsc30, Ntsc60).

#### Scenario: Rational framerate
- **WHEN** user calls `Framerate.Rational(30000, 1001)`
- **THEN** Numerator is 30000, Denominator is 1001, and ToDouble() returns ~29.97

#### Scenario: NTSC constant
- **WHEN** user references `Framerate.Ntsc30`
- **THEN** it equals `Framerate.Rational(30000, 1001)`

### Requirement: Quality as disjoint sum type
The system SHALL define `QualitySpec` as an abstract record with `ConstantQuality(int Crf)` and `ConstantBitrate(Bitrate Rate)` subtypes. A `Quality` static class SHALL provide `Crf(int)` and `Bitrate(Bitrate)` factories.

#### Scenario: CRF quality
- **WHEN** user calls `Quality.Crf(23)`
- **THEN** it returns a `ConstantQuality` with Crf=23

#### Scenario: Bitrate quality
- **WHEN** user calls `Quality.Bitrate(Bitrate.Mbps(4))`
- **THEN** it returns a `ConstantBitrate` with Rate.BitsPerSecond=4000000

#### Scenario: Cannot set both
- **WHEN** the builder exposes `.Quality(QualitySpec)`
- **THEN** calling it twice overwrites — there is no way to have both CRF and Bitrate simultaneously

### Requirement: Preset as readonly record struct
The system SHALL define `Preset` as a `readonly record struct` with `Name` property and static factories: UltraFast, SuperFast, VeryFast, Faster, Fast, Medium, Slow, Slower, VerySlow, plus `Custom(string)` for encoder-specific presets like NVENC "p5".

#### Scenario: Standard preset
- **WHEN** user references `Preset.Slow`
- **THEN** its Name is "slow"

### Requirement: VideoProfile, Tune, PixelFormat as readonly record structs
Each SHALL follow the same pattern: `readonly record struct` with `Name` property, static factories for well-known values, and `Custom(string)`.

#### Scenario: Well-known pixel format
- **WHEN** user references `PixelFormat.Yuv420p`
- **THEN** its Name is "yuv420p"

### Requirement: ContainerFormat as readonly record struct
The system SHALL define `ContainerFormat` with static factories: Mp4, Mkv, WebM, Ts, Flv, Avi, Mov, Ogg, Wav, Mp3, Flac, Null, and `Custom(string)`.

#### Scenario: Null format for analysis
- **WHEN** user references `ContainerFormat.Null`
- **THEN** its Name is "null" and the serializer emits `-f null`

### Requirement: HWAccel as readonly record struct
The system SHALL define `HWAccel` with static factories: Auto, Cuda, Qsv, Dxva2, D3d11va, VideoToolbox, Vaapi, and `Custom(string)`.

#### Scenario: HW accel constant
- **WHEN** user references `HWAccel.Cuda`
- **THEN** its Name is "cuda"

