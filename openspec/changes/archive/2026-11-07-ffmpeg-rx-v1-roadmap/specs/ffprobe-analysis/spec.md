## ADDED Requirements

### Requirement: Analyse file by path
The system SHALL provide `Ffprobe.AnalyseAsync(string path, CancellationToken ct)` that runs ffprobe and returns a `MediaInfo` record.

#### Scenario: Analyse a valid video file
- **WHEN** user calls `Ffprobe.AnalyseAsync("video.mp4")`
- **THEN** the system runs `ffprobe -print_format json -show_format -show_streams -show_chapters` and returns a populated `MediaInfo`

#### Scenario: Analyse a non-existent file
- **WHEN** user calls `Ffprobe.AnalyseAsync("missing.mp4")`
- **THEN** the system throws `FileNotFoundException`

### Requirement: Analyse URL
The system SHALL provide `Ffprobe.AnalyseAsync(Uri url, CancellationToken ct)` that probes a URL.

#### Scenario: Analyse an HTTP URL
- **WHEN** user calls `Ffprobe.AnalyseAsync(new Uri("https://example.com/video.mp4"))`
- **THEN** the system runs ffprobe against the URL and returns `MediaInfo`

### Requirement: MediaInfo record tree
`MediaInfo` SHALL be an immutable record containing: `Duration` (TimeSpan), `Bitrate` (long), `FormatName` (string), `VideoStreams` (IReadOnlyList<VideoStreamInfo>), `AudioStreams` (IReadOnlyList<AudioStreamInfo>), `SubtitleStreams` (IReadOnlyList<SubtitleStreamInfo>), `Chapters` (IReadOnlyList<ChapterInfo>), and `RawJson` (string).

#### Scenario: Access primary video stream
- **WHEN** user reads `info.VideoStreams[0]`
- **THEN** it contains `Width`, `Height`, `Codec`, `Fps`, `Bitrate`, `PixelFormat`, `Index`

#### Scenario: Access primary audio stream
- **WHEN** user reads `info.AudioStreams[0]`
- **THEN** it contains `Codec`, `SampleRate`, `Channels`, `Bitrate`, `Index`

#### Scenario: Access chapters
- **WHEN** user reads `info.Chapters`
- **THEN** each `ChapterInfo` contains `Title`, `Start` (TimeSpan), `End` (TimeSpan)

### Requirement: Raw JSON escape hatch
`MediaInfo` SHALL expose the raw ffprobe JSON output as `RawJson` so users can parse fields the library does not model.

#### Scenario: Access raw JSON
- **WHEN** user reads `info.RawJson`
- **THEN** it contains the complete ffprobe JSON output as a string

### Requirement: Custom ffprobe binary path
`Ffprobe.AnalyseAsync` SHALL accept an optional `FfprobeOptions` with a `BinaryPath` property. If not set, it SHALL use `"ffprobe"` (from PATH).

#### Scenario: Custom binary path
- **WHEN** user calls `Ffprobe.AnalyseAsync("video.mp4", new FfprobeOptions { BinaryPath = "/usr/local/bin/ffprobe" })`
- **THEN** the system runs the specified binary

### Requirement: Non-zero exit handling
The system SHALL throw `FfprobeException` with `ExitCode` and `StandardError` when ffprobe exits with a non-zero code.

#### Scenario: Corrupt file
- **WHEN** ffprobe exits with code 1 and stderr contains error details
- **THEN** the system throws `FfprobeException` containing the exit code and stderr text
