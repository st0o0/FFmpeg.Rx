## ADDED Requirements

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
