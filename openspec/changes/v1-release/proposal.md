## Why

FFmpeg.Rx has a clean typed architecture (CliWrap, PipelineSpec, scoped builders, R3 events) but is not release-ready. Missing: NuGet packaging infrastructure, common use cases (snapshot, concat), builder test coverage (0% on most builders), pipe I/O tests, and consumer documentation.

## What Changes

### Phase 1: Packaging & Infrastructure

- `global.json` — pin SDK version
- `packages.lock.json` — reproducible builds, CI locked-mode
- SourceLink — debugging support in NuGet packages
- `README.md` — usage examples, API overview, getting started
- `RELEASE_NOTES.md` — changelog
- Release workflow (`dotnet-release-nuget.yml`) from shared workflows
- NuGet icon/description in Directory.Build.props

### Phase 2: Missing Features

- **Snapshot/Thumbnail** — extract a single frame as image (`-frames:v 1`)
- **Concat/Join** — concatenate multiple videos (concat demuxer or filter)
- **Percentage Progress** — wire `WithProgressReporting()` to auto-probe duration and emit `PercentageProgressEvent`
- **Convenience API** — `Ffmpeg.Probe(path)` shortcut, `Ffmpeg.Snapshot(path, time, outputPath)`

### Phase 3: 100% Test Coverage

Unit test coverage for every public type:

**Builders at 0%:**
- `AudioEncodingBuilder`, `VideoEncodingBuilder`, `TimeRangeBuilder`, `MetadataBuilder`
- `MultiOutputBuilder`, `TeeBuilder`, `TeeTargetOptions`
- `AudioFilterBuilder`, `VideoFilterBuilder`

**Entry points at <50%:**
- `Ffmpeg` static entry (25%) — test all Input overloads (string, Uri, Stream)
- `OutputBuilder` (30%) — test all methods (Stream, Filter, Container, TimeRange, Metadata, FastStart, MovFlags, NoOverwrite, MapLabel, CustomArg)
- `PipelineBuilder` (38%) — test OutputPipe, OutputNull, OutputTee, Outputs, WithProgressReporting
- `StreamBuilder` (43%) — test AudioEncode, SubtitleEncode, Metadata, Disposition

**Untested features:**
- `PipeInputSpec` / `PipeDestination` (0%) — pipe I/O via CliWrap
- `PercentageProgressEvent` (0%) — percentage calculation
- `FfprobeException` (0%) — error path
- `CropExprFilter`, `VolumeExprFilter` (0%) — expression-based filters
- `FilterChainBuilder` (26%) — more chain combinations

**Integration test gaps:**
- Pipe input from Stream
- Pipe output to Stream
- Multi-output with different codecs
- Tee muxer
- Null output (analysis pattern)

### Phase 4: DX & Polish

- C#14 `extension()` blocks for R3 composability
- `.WithProgressReporting()` auto-wires `-progress pipe:1`
- Clean up old TeeBuilder/TeeTargetOptions (align with OutputSpec.Tee pattern)
- XML docs on public API for IntelliSense
