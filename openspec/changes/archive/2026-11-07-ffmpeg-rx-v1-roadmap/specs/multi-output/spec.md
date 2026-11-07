## ADDED Requirements

### Requirement: Multiple file outputs
`PipelineBuilder` SHALL support chaining multiple `.Output(...)` calls, each producing a separate output in a single ffmpeg run.

#### Scenario: Two file outputs
- **WHEN** user calls `.Output("hd.mp4", ...).Output("sd.mp4", ...)`
- **THEN** ArgumentBuilder emits arguments for both outputs in a single command, ffmpeg encodes once per output

### Requirement: Tee muxer output
`PipelineBuilder` SHALL provide `.OutputTee(Action<TeeBuilder> configure)` for the tee muxer (encode once, write to many).

#### Scenario: Tee to two files
- **WHEN** user configures a tee with two file targets
- **THEN** ArgumentBuilder emits `-f tee "[f=mp4]out1.mp4|[f=mp4]out2.mp4"`

### Requirement: Tee target options
Each tee target SHALL support per-target options: `ForceFormat`, `Select` (stream selection), `BitstreamFilter`, `MovFlags`, `OnFail(ignore)`.

#### Scenario: Tee with select and format
- **WHEN** user adds a tee target with `.ForceFormat("mpegts").Select("v")` 
- **THEN** the tee target renders as `[f=mpegts:select=v]path`

### Requirement: Null output
`PipelineBuilder` SHALL provide `.OutputNull(Action<OutputOptions> configure)` for analysis-only runs (`-f null -`).

#### Scenario: Null output for detection
- **WHEN** user calls `.OutputNull(o => o.AudioFilter(f => f.SilenceDetect(-30, 2)))`
- **THEN** ArgumentBuilder emits `-af "silencedetect=n=-30dB:d=2" -f null -`
