# FFmpeg.Rx

Reactive FFmpeg process wrapper using R3 Observables. Type-safe, composable, testable.

## Solution structure

```
src/
  FFmpeg.Rx/              # Main library
    Builder/              # Scoped fluent builders → produce pipeline records
    Codecs/               # Value objects: VideoCodec, AudioCodec, etc.
    Events/               # FfmpegEvent hierarchy (Observable stream)
    FilterGraph/          # Complex filter graph (DAG)
    Filters/              # Simple filter interfaces + implementations
    Internal/             # ArgumentSerializer, ProgressParser, ErrorClassifier, ProbeRunner
    Metadata/             # FFMetadata chapter builder
    Models/               # Pipeline records: PipelineConfig, InputConfig, OutputConfig, etc.
    Probing/              # ffprobe: MediaInfo, stream info records
  FFmpeg.Rx.Tests/        # xUnit v3 unit tests (no ffmpeg binary needed)
  FFmpeg.Rx.IntegrationTests/ # xUnit v3 integration tests (requires ffmpeg)
    Internal/             # FfmpegTestBase, FfmpegAssert, FfmpegFixture
    Transcode/            # Real encoding, remuxing, format conversion
    EventStream/          # Progress events, cancellation, error handling
    Probing/              # ffprobe against known test files
    Filters/              # Scale, crop verified by probing output
    PipeIO/               # Pipe input/output via CliWrap
    Advanced/             # Multi-output, containers, metadata
testdata/                 # Synthetic test media (lavfi-generated, CC0)
```

## Architecture

Three-layer design — each layer is independently testable:

```
Layer 1: Pipeline records   — what the user wants   (PipelineConfig, OutputConfig, CodecConfig...)
Layer 2: Scoped builders    — how the user says it   (PipelineBuilder, OutputBuilder, StreamBuilder...)
Layer 3: Serializer+CliWrap — how it gets done        (ArgumentSerializer, FfmpegSession via CliWrap)
```

### Core types

| Type | Role |
|------|------|
| `PipelineConfig` | Root record: inputs + outputs + global options |
| `InputConfig` | Abstract: FileInput, UrlInput, PipeInput |
| `OutputConfig` | Typed output: destination, streams, container, filters, metadata, time range |
| `StreamConfig` | Stream mapping with disjoint CodecConfig |
| `CodecConfig` | Abstract: StreamCopy, VideoEncode, AudioEncode, SubtitleEncode |
| `IFfmpegSession` | Consumer-facing interface (mockable) |
| `FfmpegSession` | Implementation: CliWrap execution, R3 event bridging |
| `ArgumentSerializer` | Pattern-matches on records → CLI string |

### Value objects (readonly record struct)

`VideoCodec`, `AudioCodec`, `SubtitleCodec`, `Bitrate`, `Framerate`, `Preset`,
`VideoProfile`, `Tune`, `PixelFormat`, `ContainerFormat`, `HWAccel` — all with
static factories and `Custom()` escape hatch.

### Disjoint sum types

- `CodecConfig` → StreamCopy | VideoEncode | AudioEncode | SubtitleEncode
- `QualityConfig` → ConstantQuality(Crf) | ConstantBitrate(Rate)
- `OutputDestination` → FileDestination | PipeDestination | NullDestination
- `FilterConfig` → SimpleFilterConfig | ComplexFilterConfig

### Event stream

`FfmpegEvent` (abstract) → `StartedEvent`, `ProgressEvent`, `LogEvent`,
`CompletedEvent`, `FailedEvent`. Single `Observable<FfmpegEvent>` with typed
convenience projections (`.Progress`, `.Log`).

### One session = one process

Build → Run → Done. No reuse. `FfmpegSession` enforces single-use via `Interlocked.Exchange`.

## Architecture guardrails

- Records are the testable truth — builders are syntactic sugar
- No string bags for domain concepts — every FFmpeg concept has a .NET type
- `readonly record struct` for value objects — zero allocation, value semantics
- `sealed` on all concrete classes — unsealing is a conscious decision
- Internal constructors on session types — creation via builders or factories
- `CustomArg()` escape hatch on every builder for uncovered flags
- Filters keep existing typed record design — no changes needed
- Probing API (`Ffprobe.AnalyseAsync`) stays unchanged

## C# conventions

- Allman braces, 4-space indent
- `sealed` by default on classes
- `var` when type is apparent, `_field` for private fields
- No XML docs, no `this.` qualifier
- Enums start with `Unknown = 0`
- `dotnet format` enforced, warnings are errors
- Private constructors + static factories on value objects (WebSocket.Rx pattern)

## Test conventions

- xUnit v3 on Microsoft.Testing.Platform — run with `dotnet run`, not `dotnet test`
- **No vacuous assertions**: `Assert.NotNull(session)` as the only assertion is banned — assert concrete behavior
- **Inline timeouts on async tests**: every `[Fact]` that awaits must have a `CancellationToken` timeout
- **Assert typed values**: test that records contain the right typed data, not that strings look right
- Layer 1 tests: construct records, assert property values
- Layer 2 tests: run builder, assert produced record matches expected
- Layer 3 tests: pass known records to serializer, assert CLI string output

## Build & test

```powershell
dotnet build src/FFmpeg.Rx.slnx
dotnet format src/FFmpeg.Rx.slnx --verify-no-changes
dotnet run --project src/FFmpeg.Rx.Tests/FFmpeg.Rx.Tests.csproj
```

## Dependencies

- R3 (Cysharp) — reactive Observable implementation
- CliWrap — process execution, stdin/stdout piping
- xUnit v3 — testing on Microsoft.Testing.Platform
- System.Text.Json — ffprobe JSON parsing (source-generated)
