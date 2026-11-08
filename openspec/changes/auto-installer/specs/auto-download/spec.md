## ADDED Requirements

### Requirement: EnsureAsync checks and downloads
`Ffmpeg.EnsureAsync()` SHALL check if ffmpeg is available. If not, it SHALL download the platform-appropriate binary to the cache directory and configure FfmpegOptions to use it.

#### Scenario: ffmpeg already on PATH
- **WHEN** `Ffmpeg.EnsureAsync()` is called and ffmpeg is on PATH
- **THEN** it returns immediately without downloading

#### Scenario: ffmpeg not available
- **WHEN** `Ffmpeg.EnsureAsync()` is called and ffmpeg is not found
- **THEN** it downloads the binary and subsequent sessions use it

### Requirement: Explicit download API
`Ffmpeg.DownloadAsync(string? targetDir, CancellationToken)` SHALL download ffmpeg to the specified or default directory.

#### Scenario: Custom directory
- **WHEN** `Ffmpeg.DownloadAsync("/opt/ffmpeg")` is called
- **THEN** binaries are extracted to `/opt/ffmpeg/`

### Requirement: IsAvailable check
`Ffmpeg.IsAvailable()` SHALL return true if ffmpeg can be executed (on PATH or configured binary path).

#### Scenario: ffmpeg on PATH
- **WHEN** ffmpeg is installed and on PATH
- **THEN** `Ffmpeg.IsAvailable()` returns true

### Requirement: Platform detection
The downloader SHALL detect the current platform (Windows/Linux/macOS) and architecture (x64/arm64) via `RuntimeInformation` and select the correct download URL.

#### Scenario: Windows x64
- **WHEN** running on Windows x64
- **THEN** the gyan.dev essentials zip is downloaded

### Requirement: Binary caching
Downloaded binaries SHALL be cached in `~/.ffmpeg-rx/bin/`. Subsequent calls SHALL not re-download if the binary exists.

#### Scenario: Cached binary
- **WHEN** `EnsureAsync()` is called twice
- **THEN** the second call returns immediately without network access

### Requirement: No auto-download
`FfmpegSession.Create()` SHALL NOT automatically download ffmpeg. Download is opt-in via explicit `EnsureAsync()` or `DownloadAsync()` calls.

#### Scenario: Missing ffmpeg without EnsureAsync
- **WHEN** a session is created without calling EnsureAsync and ffmpeg is not found
- **THEN** RunAsync fails with a clear error, not an automatic download
