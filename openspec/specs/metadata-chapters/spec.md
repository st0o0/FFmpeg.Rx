# metadata-chapters Specification

## Purpose
TBD - created by archiving change ffmpeg-rx-v1-roadmap. Update Purpose after archive.
## Requirements
### Requirement: Stream metadata
`OutputOptions` SHALL provide `.StreamMetadata(string key, string value, StreamType type, int? index)`.

#### Scenario: Set stream title
- **WHEN** user calls `.StreamMetadata("title", "English", StreamType.Audio, 0)`
- **THEN** ArgumentBuilder emits `-metadata:s:a:0 title=English`

### Requirement: Global metadata
`OutputOptions` SHALL provide `.GlobalMetadata(string key, string value)`.

#### Scenario: Set title
- **WHEN** user calls `.GlobalMetadata("title", "My Video")`
- **THEN** ArgumentBuilder emits `-metadata title=My Video`

### Requirement: Chapter writing via FFMetadata
The system SHALL provide an `FFMetadataBuilder` that builds an ffmetadata file for chapter information.

#### Scenario: Write chapters
- **WHEN** user creates `new FFMetadataBuilder().WithTitle("Video").WithChapter("Intro", TimeSpan.Zero, TimeSpan.FromSeconds(30)).WithChapter("Main", TimeSpan.FromSeconds(30), TimeSpan.FromMinutes(5)).Build()`
- **THEN** it produces a valid ffmetadata format string with `[CHAPTER]` sections

### Requirement: Add metadata file to pipeline
`PipelineBuilder` SHALL provide `.AddMetadata(FFMetadataBuilder builder)` that writes the metadata to a temp file and adds it as an input with `-map_metadata`.

#### Scenario: Attach chapters to output
- **WHEN** user adds metadata builder to the pipeline
- **THEN** ArgumentBuilder emits the metadata file as an additional input and maps it

### Requirement: Disposition flags
`OutputOptions` SHALL provide `.Disposition(StreamType type, int index, params string[] flags)`.

#### Scenario: Set default audio track
- **WHEN** user calls `.Disposition(StreamType.Audio, 0, "default")`
- **THEN** ArgumentBuilder emits `-disposition:a:0 default`

#### Scenario: Set forced subtitle
- **WHEN** user calls `.Disposition(StreamType.Subtitle, 0, "forced")`
- **THEN** ArgumentBuilder emits `-disposition:s:0 forced`

### Requirement: Strip all metadata
`OutputOptions` SHALL provide `.WithoutMetadata()`.

#### Scenario: Remove metadata
- **WHEN** user calls `.WithoutMetadata()`
- **THEN** ArgumentBuilder emits `-map_metadata -1`

### Requirement: Copy metadata from input
`OutputOptions` SHALL provide `.CopyMetadataFrom(int inputIndex)`.

#### Scenario: Copy metadata
- **WHEN** user calls `.CopyMetadataFrom(0)`
- **THEN** ArgumentBuilder emits `-map_metadata 0`

