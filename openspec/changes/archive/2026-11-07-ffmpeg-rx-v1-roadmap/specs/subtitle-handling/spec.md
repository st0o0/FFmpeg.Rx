## ADDED Requirements

### Requirement: Mux subtitles
The system SHALL support adding subtitle streams to output via stream mapping with subtitle codecs.

#### Scenario: Mux SRT as mov_text
- **WHEN** user maps a subtitle stream with `.Map(1, 0, StreamType.Subtitle, SubtitleCodec.MovText)`
- **THEN** ArgumentBuilder emits `-map 1:s:0 -c:s mov_text`

### Requirement: Subtitle codec constants
The system SHALL provide a `SubtitleCodec` static class with constants: MovText, Srt, Ass, WebVtt, Copy.

#### Scenario: Use subtitle codec
- **WHEN** user references `SubtitleCodec.MovText`
- **THEN** its value is `"mov_text"`

### Requirement: Extract subtitles
The system SHALL support extracting subtitle streams by mapping only the subtitle stream to output.

#### Scenario: Extract first subtitle to SRT
- **WHEN** user creates a pipeline mapping only `StreamType.Subtitle` from input to an `.srt` output
- **THEN** ffmpeg extracts the subtitle stream to the output file

### Requirement: Burn subtitles via complex filter
Subtitle burn-in SHALL be supported via the `subtitles` filter in a complex filter chain (see complex-filters spec).

#### Scenario: Burn external SRT
- **WHEN** user adds `.BurnSubtitles("subs.srt")` in a filter chain
- **THEN** filter renders as `subtitles=subs.srt`

### Requirement: Subtitle language metadata
Users SHALL be able to set subtitle stream language via `.StreamMetadata("language", "eng", StreamType.Subtitle, 0)` (see metadata-chapters spec).

#### Scenario: Set subtitle language
- **WHEN** user sets language metadata on a subtitle stream
- **THEN** ArgumentBuilder emits `-metadata:s:s:0 language=eng`
