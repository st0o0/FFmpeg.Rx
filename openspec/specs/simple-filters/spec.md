# simple-filters Specification

## Purpose
TBD - created by archiving change ffmpeg-rx-v1-roadmap. Update Purpose after archive.
## Requirements
### Requirement: Video filter builder
`OutputOptions` SHALL provide a `.VideoFilter(Action<VideoFilterBuilder> configure)` method that builds a `-vf` filter chain from immutable filter records.

#### Scenario: Single video filter
- **WHEN** user calls `.VideoFilter(f => f.Scale(1920, 1080))`
- **THEN** ArgumentBuilder emits `-vf "scale=1920:1080"`

#### Scenario: Chained video filters
- **WHEN** user calls `.VideoFilter(f => f.Scale(1920, 1080).HorizontalFlip())`
- **THEN** ArgumentBuilder emits `-vf "scale=1920:1080,hflip"`

### Requirement: Audio filter builder
`OutputOptions` SHALL provide an `.AudioFilter(Action<AudioFilterBuilder> configure)` method that builds an `-af` filter chain.

#### Scenario: Single audio filter
- **WHEN** user calls `.AudioFilter(f => f.LoudNorm())`
- **THEN** ArgumentBuilder emits `-af "loudnorm"`

### Requirement: Scale filter
`VideoFilterBuilder` SHALL provide `.Scale(int width, int height)` and `.Scale(int value, ScaleDimension dimension)` methods. Use -2 for the auto-calculated dimension to maintain even dimensions.

#### Scenario: Scale to specific size
- **WHEN** user calls `.Scale(1920, 1080)`
- **THEN** filter renders as `scale=1920:1080`

#### Scenario: Scale width, auto height
- **WHEN** user calls `.Scale(1280, ScaleDimension.Width)`
- **THEN** filter renders as `scale=1280:-2`

### Requirement: Crop filter
`VideoFilterBuilder` SHALL provide `.Crop(int width, int height, int x, int y)` and `.Crop(string expression)`.

#### Scenario: Crop with coordinates
- **WHEN** user calls `.Crop(640, 480, 100, 50)`
- **THEN** filter renders as `crop=640:480:100:50`

### Requirement: Flip and rotate filters
`VideoFilterBuilder` SHALL provide `.HorizontalFlip()`, `.VerticalFlip()`, and `.Transpose(TransposeMode mode)`.

#### Scenario: Horizontal flip
- **WHEN** user calls `.HorizontalFlip()`
- **THEN** filter renders as `hflip`

#### Scenario: Rotate 90 clockwise
- **WHEN** user calls `.Transpose(TransposeMode.Clock)`
- **THEN** filter renders as `transpose=1`

### Requirement: Fade filter
`VideoFilterBuilder` SHALL provide `.Fade(FadeType type, int startFrame, int frames)` and `.Fade(FadeType type, TimeSpan start, TimeSpan duration)`.

#### Scenario: Fade in by frames
- **WHEN** user calls `.Fade(FadeType.In, 0, 30)`
- **THEN** filter renders as `fade=t=in:s=0:n=30`

### Requirement: Pad filter
`VideoFilterBuilder` SHALL provide `.Pad(int width, int height, int x, int y, string color)`.

#### Scenario: Pad to 1920x1080 centered
- **WHEN** user calls `.Pad(1920, 1080, -1, -1, "black")`
- **THEN** filter renders as `pad=1920:1080:-1:-1:black`

### Requirement: Deinterlace filter
`VideoFilterBuilder` SHALL provide `.Yadif()`.

#### Scenario: Deinterlace
- **WHEN** user calls `.Yadif()`
- **THEN** filter renders as `yadif`

### Requirement: Volume filter
`AudioFilterBuilder` SHALL provide `.Volume(double factor)` and `.Volume(string expression)`.

#### Scenario: Set volume to 80%
- **WHEN** user calls `.Volume(0.8)`
- **THEN** filter renders as `volume=0.8`

### Requirement: Loudness normalization
`AudioFilterBuilder` SHALL provide `.LoudNorm()` with optional target parameters.

#### Scenario: Default loudnorm
- **WHEN** user calls `.LoudNorm()`
- **THEN** filter renders as `loudnorm`

#### Scenario: Custom loudnorm
- **WHEN** user calls `.LoudNorm(i: -16, tp: -1.5, lra: 11)`
- **THEN** filter renders as `loudnorm=I=-16:TP=-1.5:LRA=11`

### Requirement: Audio resample
`AudioFilterBuilder` SHALL provide `.AResample(int sampleRate)`.

#### Scenario: Resample to 48kHz
- **WHEN** user calls `.AResample(48000)`
- **THEN** filter renders as `aresample=48000`

### Requirement: Silence detect
`AudioFilterBuilder` SHALL provide `.SilenceDetect(double noiseDb, double duration)`.

#### Scenario: Detect silence
- **WHEN** user calls `.SilenceDetect(-30, 2.0)`
- **THEN** filter renders as `silencedetect=n=-30dB:d=2`

### Requirement: Custom filter escape hatch
Both builders SHALL provide a `.Custom(string name, string? args)` method for any filter not modeled.

#### Scenario: Custom video filter
- **WHEN** user calls `.Custom("unsharp", "5:5:1.0")`
- **THEN** filter renders as `unsharp=5:5:1.0`

#### Scenario: Custom filter without args
- **WHEN** user calls `.Custom("pp")`
- **THEN** filter renders as `pp`

