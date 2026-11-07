# time-control Specification

## Purpose
TBD - created by archiving change ffmpeg-rx-v1-roadmap. Update Purpose after archive.
## Requirements
### Requirement: Input seek
`InputOptions` SHALL provide a `.Seek(TimeSpan position)` method that places `-ss` before `-i` for fast seeking.

#### Scenario: Seek input
- **WHEN** user calls `.Seek(TimeSpan.FromSeconds(30))` on input options
- **THEN** ArgumentBuilder emits `-ss 00:00:30.000` before the `-i` argument

### Requirement: Output seek
`OutputOptions` SHALL provide a `.Seek(TimeSpan position)` method that places `-ss` after input arguments for frame-accurate seeking.

#### Scenario: Output seek
- **WHEN** user calls `.Seek(TimeSpan.FromSeconds(30))` on output options
- **THEN** ArgumentBuilder emits `-ss 00:00:30.000` in the output arguments

### Requirement: Duration limit
`OutputOptions` SHALL provide a `.Duration(TimeSpan length)` method.

#### Scenario: Limit duration
- **WHEN** user calls `.Duration(TimeSpan.FromSeconds(10))`
- **THEN** ArgumentBuilder emits `-t 10.000`

### Requirement: Stop time
`OutputOptions` SHALL provide a `.StopTime(TimeSpan position)` method.

#### Scenario: Set stop time
- **WHEN** user calls `.StopTime(TimeSpan.FromMinutes(1))`
- **THEN** ArgumentBuilder emits `-to 00:01:00.000`

### Requirement: Input loop
`InputOptions` SHALL provide a `.Loop()` method for looping a still image input.

#### Scenario: Loop input
- **WHEN** user calls `.Loop()` on input options
- **THEN** ArgumentBuilder emits `-loop 1` before `-i`

### Requirement: Stream loop
`InputOptions` SHALL provide a `.StreamLoop(int count)` method. Pass -1 for infinite loop.

#### Scenario: Loop input stream 3 times
- **WHEN** user calls `.StreamLoop(3)` on input options
- **THEN** ArgumentBuilder emits `-stream_loop 3` before `-i`

### Requirement: Video speed change
`OutputOptions` SHALL provide a `.VideoSpeed(double factor)` method using the `setpts` filter internally.

#### Scenario: Double speed
- **WHEN** user calls `.VideoSpeed(2.0)`
- **THEN** ArgumentBuilder emits `-filter:v "setpts=0.5*PTS"`

### Requirement: Audio tempo change
`OutputOptions` SHALL provide an `.AudioTempo(double factor)` method using the `atempo` filter.

#### Scenario: Double tempo
- **WHEN** user calls `.AudioTempo(2.0)`
- **THEN** ArgumentBuilder emits `-filter:a "atempo=2.0"`

#### Scenario: Extreme tempo chains
- **WHEN** user calls `.AudioTempo(4.0)` (exceeds atempo's 0.5-2.0 range)
- **THEN** ArgumentBuilder chains filters: `-filter:a "atempo=2.0,atempo=2.0"`

