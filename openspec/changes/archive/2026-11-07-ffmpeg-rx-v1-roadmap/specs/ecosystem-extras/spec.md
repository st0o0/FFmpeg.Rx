## ADDED Requirements

### Requirement: Thumbnail sheet
`OutputOptions` SHALL support generating a thumbnail sheet using the tile filter with select and scale.

#### Scenario: Generate 5x5 thumbnail sheet
- **WHEN** user builds a pipeline with video filters for select (every N frames), scale, and tile(5x5) targeting a `.jpg` output
- **THEN** ArgumentBuilder emits the appropriate `-vf "select=...,scale=...,tile=5x5"` arguments

### Requirement: Image sequence input
`InputOptions` SHALL provide `.Framerate(double fps)` for setting input framerate, and `PipelineBuilder.Input(string globPattern)` SHALL accept image sequence patterns (e.g. `"frames/img_%04d.png"`).

#### Scenario: Image sequence to video
- **WHEN** user calls `Ffmpeg.Input("frames/img_%04d.png", o => o.Framerate(30))`
- **THEN** ArgumentBuilder emits `-framerate 30 -i "frames/img_%04d.png"`

### Requirement: Negative mapping
`OutputOptions` SHALL provide `.NegativeMap(int inputIndex, StreamType type)` to exclude streams.

#### Scenario: Exclude audio
- **WHEN** user calls `.NegativeMap(0, StreamType.Audio)`
- **THEN** ArgumentBuilder emits `-map -0:a`

### Requirement: Codec tags
`OutputOptions` SHALL provide `.CodecTag(string tag, StreamType type)`.

#### Scenario: Set HEVC tag for Apple compatibility
- **WHEN** user calls `.CodecTag("hvc1", StreamType.Video)`
- **THEN** ArgumentBuilder emits `-tag:v hvc1`

### Requirement: MovFlags
`OutputOptions` SHALL provide `.MovFlags(params string[] flags)`.

#### Scenario: FastStart
- **WHEN** user calls `.MovFlags("faststart")`
- **THEN** ArgumentBuilder emits `-movflags faststart`

#### Scenario: Fragmented MP4
- **WHEN** user calls `.MovFlags("frag_keyframe", "empty_moov")`
- **THEN** ArgumentBuilder emits `-movflags frag_keyframe+empty_moov`

### Requirement: Map all streams from input
`OutputOptions` SHALL provide `.MapAll(int inputIndex)` to map all streams from an input.

#### Scenario: Map everything
- **WHEN** user calls `.MapAll(0)`
- **THEN** ArgumentBuilder emits `-map 0`

### Requirement: Copy all streams
`OutputOptions` SHALL provide `.CopyStreams()` to copy all streams without re-encoding.

#### Scenario: Remux
- **WHEN** user calls `.CopyStreams()`
- **THEN** ArgumentBuilder emits `-c copy`

### Requirement: Encoder tune
`OutputOptions` SHALL provide `.Tune(string tune)`. A `EncoderTune` static class SHALL define constants (Film, Animation, Grain, StillImage, FastDecode, ZeroLatency).

#### Scenario: Set tune
- **WHEN** user calls `.Tune(EncoderTune.Film)`
- **THEN** ArgumentBuilder emits `-tune film`
