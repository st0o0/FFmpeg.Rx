## ADDED Requirements

### Requirement: FilterGraph creation
The system SHALL provide a `FilterGraph` class with a `Create()` static method returning a `FilterGraphBuilder`.

#### Scenario: Create empty graph
- **WHEN** user calls `FilterGraph.Create()`
- **THEN** a `FilterGraphBuilder` is returned that can chain inputs, filters, and merges

### Requirement: Label graph inputs
`FilterGraphBuilder` SHALL provide `.Input(int inputIndex, StreamType type, string label)` to label an input stream pad.

#### Scenario: Label video input
- **WHEN** user calls `.Input(0, StreamType.Video, "base")`
- **THEN** the graph maps `[0:v]` to label `base`

### Requirement: Single-input filter chain
`FilterGraphBuilder` SHALL provide `.Chain(string inputLabel, Action<FilterChainBuilder> configure, string outputLabel)` for linear filter chains.

#### Scenario: Scale chain
- **WHEN** user calls `.Chain("base", f => f.Scale(1920, 1080), "scaled")`
- **THEN** the graph serializes as `[base]scale=1920:1080[scaled]`

### Requirement: Multi-input merge
`FilterGraphBuilder` SHALL provide `.Merge(string label1, string label2, Action<MergeFilterBuilder> configure, string outputLabel)` for filters requiring multiple inputs.

#### Scenario: Overlay merge
- **WHEN** user calls `.Merge("scaled", "logo", f => f.Overlay(10, 10), "out")`
- **THEN** the graph serializes as `[scaled][logo]overlay=10:10[out]`

### Requirement: Concat filter
`MergeFilterBuilder` SHALL provide `.Concat(int segments, bool hasVideo, bool hasAudio)`.

#### Scenario: Concat two video+audio segments
- **WHEN** user calls `.Concat(2, true, true)`
- **THEN** filter renders as `concat=n=2:v=1:a=1`

### Requirement: Overlay filter
`MergeFilterBuilder` SHALL provide `.Overlay(int x, int y)` and `.Overlay(string xExpr, string yExpr)`.

#### Scenario: Overlay at position
- **WHEN** user calls `.Overlay(10, 10)`
- **THEN** filter renders as `overlay=10:10`

#### Scenario: Overlay centered
- **WHEN** user calls `.Overlay("(W-w)/2", "(H-h)/2")`
- **THEN** filter renders as `overlay=(W-w)/2:(H-h)/2`

### Requirement: Audio mix
`MergeFilterBuilder` SHALL provide `.AudioMix()` for the amix filter.

#### Scenario: Mix two audio streams
- **WHEN** user calls `.AudioMix()` in a merge of two audio labels
- **THEN** filter renders as `amix=inputs=2`

### Requirement: Split / ASplit
`FilterGraphBuilder` SHALL provide `.Split(string inputLabel, string outputLabel1, string outputLabel2)` and `.ASplit(...)`.

#### Scenario: Split video stream
- **WHEN** user calls `.Split("base", "copy1", "copy2")`
- **THEN** the graph serializes as `[base]split[copy1][copy2]`

### Requirement: Map filter graph output
`OutputOptions` SHALL provide `.MapLabel(string label)` to map a labeled filter graph output to the output file.

#### Scenario: Map labeled output
- **WHEN** user calls `.MapLabel("out")` and a `FilterComplex(graph)` is set
- **THEN** ArgumentBuilder emits `-map "[out]"`

### Requirement: FilterComplex on OutputOptions
`OutputOptions` SHALL provide `.FilterComplex(FilterGraph graph)` that serializes the graph as `-filter_complex`.

#### Scenario: Full complex filter pipeline
- **WHEN** graph has `[0:v]scale=1920:1080[scaled];[scaled][1:v]overlay=10:10[out]`
- **THEN** ArgumentBuilder emits `-filter_complex "[0:v]scale=1920:1080[scaled];[scaled][1:v]overlay=10:10[out]"`

### Requirement: Subtitle burn-in via subtitles filter
`FilterChainBuilder` SHALL provide `.BurnSubtitles(string path)` using the ffmpeg `subtitles` filter.

#### Scenario: Burn subtitles
- **WHEN** user calls `.BurnSubtitles("subs.srt")` in a chain
- **THEN** filter renders as `subtitles=subs.srt`
