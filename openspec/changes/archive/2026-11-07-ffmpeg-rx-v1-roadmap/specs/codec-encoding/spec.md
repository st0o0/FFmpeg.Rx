## ADDED Requirements

### Requirement: Typed video codec constants
The system SHALL provide a `VideoCodec` static class with string constants for common video encoders (LibX264, LibX265, LibVpx, LibVpxVp9, LibAomAv1, LibSvtAv1, Copy, and others). The builder SHALL accept any `string` as a codec, not just the predefined constants.

#### Scenario: Use predefined video codec
- **WHEN** user calls `.Map(0, 0, StreamType.Video, VideoCodec.LibX264)`
- **THEN** ArgumentBuilder emits `-c:v libx264`

#### Scenario: Use custom codec string
- **WHEN** user calls `.Map(0, 0, StreamType.Video, "libsvtav1")`
- **THEN** ArgumentBuilder emits `-c:v libsvtav1`

### Requirement: Typed audio codec constants
The system SHALL provide an `AudioCodec` static class with string constants for common audio encoders (Aac, LibOpus, LibMp3Lame, Flac, Pcm16Le, Copy, and others).

#### Scenario: Use predefined audio codec
- **WHEN** user calls `.Map(0, 0, StreamType.Audio, AudioCodec.Aac)`
- **THEN** ArgumentBuilder emits `-c:a aac`

### Requirement: Constant rate factor (CRF)
`OutputOptions` SHALL provide a `.Crf(int value)` method that sets the constant rate factor for quality-based encoding.

#### Scenario: Set CRF
- **WHEN** user calls `.Crf(23)`
- **THEN** ArgumentBuilder emits `-crf 23`

### Requirement: Video bitrate
`OutputOptions` SHALL provide a `.VideoBitrate(string rate)` method.

#### Scenario: Set video bitrate
- **WHEN** user calls `.VideoBitrate("4M")`
- **THEN** ArgumentBuilder emits `-b:v 4M`

### Requirement: Audio bitrate
`OutputOptions` SHALL provide an `.AudioBitrate(string rate)` method.

#### Scenario: Set audio bitrate
- **WHEN** user calls `.AudioBitrate("192k")`
- **THEN** ArgumentBuilder emits `-b:a 192k`

### Requirement: Framerate
`OutputOptions` SHALL provide a `.Framerate(double fps)` and `.Framerate(string rational)` method.

#### Scenario: Set numeric framerate
- **WHEN** user calls `.Framerate(30)`
- **THEN** ArgumentBuilder emits `-r 30`

#### Scenario: Set rational framerate
- **WHEN** user calls `.Framerate("30000/1001")`
- **THEN** ArgumentBuilder emits `-r 30000/1001`

### Requirement: Encoder preset
`OutputOptions` SHALL provide a `.Preset(string preset)` method. A `Preset` static class SHALL define common constants (UltraFast, SuperFast, VeryFast, Faster, Fast, Medium, Slow, Slower, VerySlow).

#### Scenario: Set encoder preset
- **WHEN** user calls `.Preset(Preset.Slow)`
- **THEN** ArgumentBuilder emits `-preset slow`

### Requirement: Encoder profile
`OutputOptions` SHALL provide a `.Profile(string profile)` method. A `VideoProfile` static class SHALL define common constants (Baseline, Main, High, High10, High422, High444).

#### Scenario: Set encoder profile
- **WHEN** user calls `.Profile(VideoProfile.High)`
- **THEN** ArgumentBuilder emits `-profile:v high`

### Requirement: Pixel format
`OutputOptions` SHALL provide a `.PixelFormat(string format)` method. A `PixelFormat` static class SHALL define common constants (Yuv420p, Yuv422p, Yuv444p, Rgb24, and others).

#### Scenario: Set pixel format
- **WHEN** user calls `.PixelFormat(PixelFormat.Yuv420p)`
- **THEN** ArgumentBuilder emits `-pix_fmt yuv420p`

### Requirement: Force output format
`OutputOptions` SHALL provide a `.ForceFormat(string format)` method.

#### Scenario: Force mpegts format
- **WHEN** user calls `.ForceFormat("mpegts")`
- **THEN** ArgumentBuilder emits `-f mpegts`

### Requirement: Overwrite control
`OutputOptions` SHALL provide a `.NoOverwrite()` method. Default behavior SHALL be `-y` (overwrite).

#### Scenario: Default overwrite
- **WHEN** user does not call `.NoOverwrite()`
- **THEN** ArgumentBuilder emits `-y` before output path

#### Scenario: Prevent overwrite
- **WHEN** user calls `.NoOverwrite()`
- **THEN** ArgumentBuilder emits `-n` instead of `-y` before output path
