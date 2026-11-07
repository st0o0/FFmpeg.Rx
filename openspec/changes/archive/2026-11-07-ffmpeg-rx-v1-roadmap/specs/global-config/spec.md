## ADDED Requirements

### Requirement: Global FFmpeg options
The system SHALL provide `FfmpegOptions` record with `BinaryPath` (string?), `FfprobeBinaryPath` (string?), `TempDirectory` (string?), `WorkingDirectory` (string?).

#### Scenario: Configure global options
- **WHEN** user calls `FfmpegOptions.Configure(new FfmpegOptions { BinaryPath = "/opt/ffmpeg/bin/ffmpeg" })`
- **THEN** all subsequent `Ffmpeg` and `Ffprobe` calls use the configured binary path

### Requirement: Per-session options override
`PipelineBuilder` SHALL accept options that override global configuration for that session.

#### Scenario: Override binary for one session
- **WHEN** user calls `Ffmpeg.At("/usr/local/bin/ffmpeg")` (existing API)
- **THEN** that session uses the specified binary regardless of global config

### Requirement: Binary discovery from PATH
When no binary path is configured, the system SHALL resolve `ffmpeg` and `ffprobe` from the system PATH. The system SHALL check platform-specific common locations as fallback.

#### Scenario: ffmpeg on PATH
- **WHEN** no binary path is configured and `ffmpeg` is on PATH
- **THEN** the system finds and uses it

#### Scenario: ffmpeg not found
- **WHEN** no binary path is configured and `ffmpeg` is not on PATH or common locations
- **THEN** the system throws a descriptive exception when a session is run

### Requirement: Working directory
`FfmpegOptions` SHALL support a `WorkingDirectory` property that sets the process working directory.

#### Scenario: Set working directory
- **WHEN** `WorkingDirectory` is set to `"/data/media"`
- **THEN** the ffmpeg process starts with that working directory

### Requirement: Optional binary auto-download
The system SHALL provide an optional extension package for downloading ffmpeg binaries at runtime using the ffbinaries API. This capability is additive and not required for core functionality.

#### Scenario: Download binaries
- **WHEN** user calls the auto-download API with a target directory
- **THEN** the system downloads ffmpeg and ffprobe for the current platform to that directory
