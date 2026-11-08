# test-infrastructure Specification

## Purpose
Test infrastructure — separate integration test project, base classes, assertion helpers, ffmpeg availability checks, timeouts, and synthetic test media.
## Requirements
### Requirement: Separate integration test project
The solution SHALL contain `FFmpeg.Rx.IntegrationTests` as a separate project from `FFmpeg.Rx.Tests`, added to the solution file.

#### Scenario: Project exists in solution
- **WHEN** the solution is built
- **THEN** both `FFmpeg.Rx.Tests` and `FFmpeg.Rx.IntegrationTests` compile

### Requirement: FfmpegTestBase with IAsyncLifetime
An abstract `FfmpegTestBase` class SHALL implement `IAsyncLifetime`, create an isolated temp output directory per test in `InitializeAsync`, and delete it in `DisposeAsync`. It SHALL provide `TestAsset(name)` and `OutputPath(name)` helpers.

#### Scenario: Test isolation
- **WHEN** two tests run in parallel
- **THEN** each has its own output directory with no file conflicts

### Requirement: FfmpegAssert with probing-based assertions
A static `FfmpegAssert` class SHALL provide assertions that probe output files via `Ffprobe.AnalyseAsync`:
- `HasVideoStream(path, expectedCodec)`
- `HasAudioStream(path, expectedCodec)`
- `HasResolution(path, width, height)`
- `HasDurationApprox(path, expected, toleranceSec)`
- `OutputExists(path)` (file exists and is non-empty)

#### Scenario: Assert video stream
- **WHEN** `FfmpegAssert.HasVideoStream("out.mp4", "h264")` is called on a valid H264 file
- **THEN** it passes without throwing

#### Scenario: Assert wrong codec fails
- **WHEN** `FfmpegAssert.HasVideoStream("out.mp4", "hevc")` is called on an H264 file
- **THEN** it throws an assertion failure

### Requirement: Skip when ffmpeg unavailable
A class fixture SHALL check for ffmpeg availability once. If ffmpeg is not on PATH, all integration tests SHALL skip with a clear message.

#### Scenario: No ffmpeg on PATH
- **WHEN** ffmpeg is not installed
- **THEN** all integration tests report as skipped, not failed

### Requirement: Timeouts on all async tests
Every async integration test SHALL have `[Fact(Timeout = 30000)]` and pass `TestContext.Current.CancellationToken` through async chains.

#### Scenario: Hanging test
- **WHEN** a test exceeds 30 seconds
- **THEN** it is cancelled and reported as failed with a timeout

### Requirement: Synthetic test media in repository
The repository SHALL contain a `testdata/` directory with synthetic media files generated via ffmpeg's lavfi virtual input. Files SHALL be small (320x240, 3-5s) and committed directly (no LFS).

#### Scenario: Test media exists
- **WHEN** the repository is cloned
- **THEN** `testdata/` contains all required test files ready to use

### Requirement: License documentation
`testdata/README.md` SHALL document that files are synthetic (lavfi-generated), include the generation commands, and state a CC0 license.

#### Scenario: License clarity
- **WHEN** a developer inspects testdata/
- **THEN** README.md explains the origin and license of every file

### Requirement: Generation script
A `testdata/generate-testdata.sh` script SHALL reproduce all test files from scratch using only ffmpeg lavfi inputs.

#### Scenario: Reproducibility
- **WHEN** the script is run with ffmpeg installed
- **THEN** it regenerates all test files identically

### Requirement: Test file inventory
The following files SHALL exist:
- `video_h264_aac_5s.mp4` — video+audio, H264+AAC, 5 seconds
- `video_only_3s.mp4` — video only, H264, 3 seconds
- `audio_only_3s.m4a` — audio only, AAC, 3 seconds
- `video_hevc_3s.mp4` — video only, HEVC, 3 seconds
- `subtitle.srt` — SRT subtitle file with 3 timed entries
- `two_audio_tracks.mkv` — video + 2 audio streams, 3 seconds
- `corrupt.mp4` — intentionally truncated file for error tests

#### Scenario: Video test file has expected properties
- **WHEN** `video_h264_aac_5s.mp4` is probed
- **THEN** it has one H264 video stream (320x240) and one AAC audio stream, duration ~5s

