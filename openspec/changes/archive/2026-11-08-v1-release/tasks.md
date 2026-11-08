## 1. Packaging & Infrastructure

- [x] 1.1 Create `src/global.json` with SDK version pin
- [x] 1.2 Generate `packages.lock.json` for all projects (`dotnet restore --use-lock-file`)
- [x] 1.3 Add `Microsoft.SourceLink.GitHub` to Directory.Packages.props and FFmpeg.Rx.csproj
- [x] 1.4 Add NuGet metadata to Directory.Build.props (Description, PackageTags)
- [x] 1.5 Write `README.md` with: description, install, quick start, builder API, probing, events
- [x] 1.6 Create `RELEASE_NOTES.md` with v1.0.0 section
- [x] 1.7 Add release workflow: `.github/workflows/release.yml` using `dotnet-release-nuget.yml`
- [x] 1.8 Verify `dotnet pack` produces valid NuGet package
- [x] 1.9 Update CI `locked-mode: true` now that lock files exist

## 2. Snapshot & Concat

- [x] 2.1 Implement `Ffmpeg.Snapshot(string input, TimeSpan time, string output)` static method
- [x] 2.2 Implement `Ffmpeg.Concat(IEnumerable<string> inputs, string output)` static method (concat demuxer, stream copy)
- [x] 2.3 Implement `Ffmpeg.Probe(string path)` shortcut for `Ffprobe.AnalyseAsync`
- [x] 2.4 Unit tests for Snapshot argument serialization
- [x] 2.5 Unit tests for Concat (temp file creation, argument format)
- [x] 2.6 Integration test: Snapshot extracts a frame, output is valid image
- [x] 2.7 Integration test: Concat joins two test files, output duration is sum
- [x] 2.8 Integration test: Probe shortcut returns same result as Ffprobe.AnalyseAsync

## 3. Percentage Progress

- [x] 3.1 Update `PipelineBuilder.WithProgressReporting(TimeSpan? knownDuration = null)` to store duration
- [x] 3.2 Update `PipelineSpec` to carry optional `TotalDuration` for percentage calculation
- [x] 3.3 Implement auto-probe in `FfmpegSession`: if TotalDuration is null and single FileInputSpec, probe before run
- [x] 3.4 Emit `PercentageProgressEvent` alongside `ProgressEvent` when TotalDuration is known
- [x] 3.5 Unit test: PercentageProgressEvent calculation (timeUs / totalDurationUs * 100)
- [x] 3.6 Integration test: transcode with WithProgressReporting emits PercentageProgressEvents

## 4. Builder Unit Tests (0% → 100%)

- [x] 4.1 `VideoEncodingBuilderTests` — Quality, Preset, Profile, Tune, PixelFormat, Framerate → assert on VideoEncodingOptions
- [x] 4.2 `AudioEncodingBuilderTests` — Bitrate, SampleRate, Channels → assert on AudioEncodingOptions
- [x] 4.3 `TimeRangeBuilderTests` — Seek, Duration, StopTime → assert on TimeRange record
- [x] 4.4 `MetadataBuilderTests` — Global, StripAll, CopyFrom → assert on MetadataSpec record
- [x] 4.5 `StreamBuilderTests` — Copy, Encode(Video), Encode(Audio), Encode(Subtitle), Metadata, Disposition → assert on StreamSpec
- [x] 4.6 `OutputBuilderTests` — all methods: Stream, VideoFilter, AudioFilter, FilterComplex, Container, TimeRange, Metadata, FastStart, MovFlags, NoOverwrite, MapLabel, CustomArg → assert on OutputSpec
- [x] 4.7 `TeeBuilderTests` — add targets with options → assert on TeeOutput/TeeTarget records
- [x] 4.8 `MultiOutputBuilderTests` — add multiple outputs → assert Build() produces correct PipelineSpec

## 5. Fluent API Tests (25-43% → 100%)

- [x] 5.1 `Ffmpeg.Input(string)` → produces PipelineBuilder with FileInputSpec
- [x] 5.2 `Ffmpeg.Input(Uri)` → produces PipelineBuilder with UrlInputSpec
- [x] 5.3 `Ffmpeg.Input(Stream, format)` → produces PipelineBuilder with PipeInputSpec
- [x] 5.4 `Ffmpeg.At(path).Input(...)` → custom binary path propagates to PipelineSpec
- [x] 5.5 `PipelineBuilder.Output(path, configure)` → IFfmpegSession with correct PipelineSpec
- [x] 5.6 `PipelineBuilder.OutputPipe(stream, configure)` → PipeDestination in OutputSpec
- [x] 5.7 `PipelineBuilder.OutputNull(configure)` → NullDestination in OutputSpec
- [x] 5.8 `PipelineBuilder.OutputTee(configure, tee)` → TeeOutput in OutputSpec
- [x] 5.9 `PipelineBuilder.Outputs().Output(...).Output(...).Build()` → multiple OutputSpecs
- [x] 5.10 `PipelineBuilder.WithProgressReporting()` → global options contain `-progress pipe:1`

## 6. Filter Builder Tests (0% → 100%)

- [x] 6.1 `VideoFilterBuilderTests` — Scale, Crop, HFlip, VFlip, Transpose, Fade, Pad, Yadif, Custom
- [x] 6.2 `AudioFilterBuilderTests` — Volume, LoudNorm, AResample, SilenceDetect, Custom
- [x] 6.3 `FilterChainBuilderTests` — chain multiple filters, build → assert on filter list
- [x] 6.4 `CropExprFilterTests` — expression-based crop Render()
- [x] 6.5 `VolumeExprFilterTests` — expression-based volume Render()

## 7. Error Path & Edge Case Tests

- [x] 7.1 `FfprobeExceptionTests` — probe with nonexistent binary → FfprobeException
- [x] 7.2 `FfprobeExceptionTests` — probe with invalid file → FfprobeException
- [x] 7.3 `ErrorClassifierTests` — additional edge cases for transient/permanent classification
- [x] 7.4 `FfmpegSession` double-run → InvalidOperationException (already tested, verify coverage)
- [x] 7.5 `FfmpegOptions` — all resolution paths (per-session, global, PATH, common locations)
- [x] 7.6 `ProgressExtensions.AsPercentage` and `WithPercentage` — R3 operator tests

## 8. Integration Test Gaps

- [x] 8.1 Pipe input: MemoryStream with raw audio → ffmpeg → output file
- [x] 8.2 Pipe output: input file → ffmpeg → MemoryStream
- [x] 8.3 Multi-output: same input → two outputs with different codecs
- [x] 8.4 Tee muxer: tee split to two files
- [x] 8.5 Null output: analysis pattern (-f null /dev/null) with filter detection
- [x] 8.6 Fluent API integration: `Ffmpeg.Input(...).Output(...)` end-to-end transcode

## 9. DX & Polish

- [x] 9.1 Create `Extensions.cs` with C#14 `extension(IFfmpegSession)` block for PercentageProgress
- [x] 9.2 Align TeeBuilder — remove TeeTargetOptions, use TeeTarget record directly
- [x] 9.3 Clean up `FfmpegOptions` — verify binary resolution chain works on all platforms

## 10. Finalize

- [x] 10.1 Run coverage: `dotnet-coverage collect` for both test projects
- [x] 10.2 Verify FFmpeg.Rx library coverage ≥ 95% (100% goal, 95% minimum)
- [x] 10.3 `dotnet build` — zero errors, zero warnings
- [x] 10.4 `dotnet format --verify-no-changes` — clean
- [x] 10.5 All unit tests pass (target: 250+)
- [x] 10.6 All integration tests pass (target: 30+)
- [x] 10.7 Update AGENTS.md with final architecture and feature list
- [x] 10.8 Final `dotnet pack` verification
