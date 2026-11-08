## Context

FFmpeg.Rx wraps ffmpeg as an external process. Unit tests validate the typed model and serializer but never execute ffmpeg. Integration tests must run real processes against real media files.

Key difference from WebSocket.Rx: WebSocket.Rx controls both sides (in-process server + client). FFmpeg.Rx controls only the wrapper — the binary is external. This means tests need actual media files as input and must verify output files via ffprobe.

## Decisions

### D1: Separate test project

`FFmpeg.Rx.IntegrationTests` as a separate csproj in the solution. Rationale: different timeout characteristics (30s vs 5s), different CI requirements (needs ffmpeg binary), different test patterns (file I/O, process management).

Following WebSocket.Rx's structure: `UnitTests` for fast pure-logic tests, `IntegrationTests` for real process tests.

### D2: Synthetic test media checked into repo

Generate test files using ffmpeg's `lavfi` virtual input (`testsrc`, `sine`). These are algorithmic patterns — no copyrighted content, no license issues. Commit them under `testdata/` at the repo root.

Files are small (320x240, 3-5 seconds, ultrafast preset) — total ~260KB, no Git LFS needed.

A `generate-testdata.sh` script documents exactly how each file was created for reproducibility. But the files themselves are committed so CI doesn't need to generate them.

**Alternative considered:** Generate at test-time in a fixture. Rejected — adds ffmpeg dependency to test setup, makes failures harder to diagnose, and wastes CI time regenerating identical files.

**Alternative considered:** Download from external URL. Rejected — flaky CI, external dependency, harder to reproduce.

### D3: Probing-based assertions

Instead of asserting on CLI strings or process output, assert on the **actual output file** using ffprobe:

```csharp
await FfmpegAssert.HasVideoStream(outputPath, "h264");
await FfmpegAssert.HasResolution(outputPath, 320, 240);
await FfmpegAssert.HasDurationApprox(outputPath, TimeSpan.FromSeconds(5), tolerance: 0.5);
```

This is the strongest possible assertion — it verifies that ffmpeg actually produced the right output, not just that we passed the right flags.

`FfmpegAssert` uses the existing `Ffprobe.AnalyseAsync` internally, so we're also testing the probing path.

### D4: Test base class with IAsyncLifetime

Following WebSocket.Rx's `ReactiveWebSocketClientTestBase` pattern:

```csharp
public abstract class FfmpegTestBase : IAsyncLifetime
{
    // Creates temp output dir per test, cleans up after
    // Provides TestAsset(name) and OutputPath(name) helpers
}
```

Each test gets an isolated temp directory. Cleanup is automatic via `DisposeAsync`.

### D5: Skip when ffmpeg unavailable

A class-level fixture checks for ffmpeg once. If missing, all integration tests skip. No per-test `if (!FfmpegAvailable())` checks.

### D6: Test organization by concern

```
Transcode/     — encoding, remuxing, format conversion
EventStream/   — progress events, cancellation, errors
Probing/       — ffprobe against known test files
Filters/       — scale, crop verified by probing output
PipeIO/        — named pipe input/output
```

### D7: Timeouts

Following WebSocket.Rx conventions: `[Fact(Timeout = 30000)]` on all async tests, `TestContext.Current.CancellationToken` through all async chains.

## Test media inventory

| File | Content | Codec | Duration | ~Size |
|------|---------|-------|----------|-------|
| `video_h264_aac_5s.mp4` | testsrc + sine 440Hz | H264+AAC | 5s | 80KB |
| `video_only_3s.mp4` | testsrc | H264 | 3s | 30KB |
| `audio_only_3s.m4a` | sine 440Hz | AAC | 3s | 20KB |
| `video_hevc_3s.mp4` | testsrc | HEVC | 3s | 25KB |
| `subtitle.srt` | 3 timed entries | SRT text | 5s | <1KB |
| `two_audio_tracks.mkv` | testsrc + 2x sine | H264+2xAAC | 3s | 60KB |
| `corrupt.mp4` | truncated bytes | — | — | 1KB |

## Risks

- **ffmpeg version differences** → Tests should assert on properties (has video stream, has resolution) not exact byte sizes or encoding details. Tolerance on duration assertions.
- **Platform differences** → Named pipe tests are Windows-only (already gated by `OperatingSystem.IsWindows()`). File path handling via `Path.Combine`.
- **CI timing** → ultrafast preset + 320x240 + 3-5s duration keeps each transcode under 2s. Full suite target: <60s.
