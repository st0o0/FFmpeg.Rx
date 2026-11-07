## Context

FFmpeg.Rx wraps ffmpeg processes as R3 `Observable<FfmpegEvent>` streams. The current architecture:

```
Ffmpeg (static entry)
  → PipelineBuilder (fluent API)
    → FfmpegPipeline (immutable records)
      → FfmpegSession (one process, Observable lifecycle)
        → ProcessRunner (internal, Process.Start + R3 Subjects)
```

All processing options are immutable records. The builder is syntactic sugar — records are the testable truth. `ArgumentBuilder` serializes records to ffmpeg CLI arguments. This pattern must hold for all new features.

Reference libraries: FFMpegCore (feature-rich, ~2k stars) and FFmpeg.NET (simpler, event-based). Goal is FFMpegCore feature parity with FFmpeg.Rx's reactive architecture.

## Goals / Non-Goals

**Goals:**
- Feature parity with FFMpegCore's builder/processing/probing capabilities
- Pure builder API — no convenience helper methods (no `FFMpeg.Snapshot()` equivalents)
- All new features expressed as immutable records serialized by `ArgumentBuilder`
- Each phase independently shippable with tests

**Non-Goals:**
- SkiaSharp/System.Drawing image frame adapters (extension package later)
- GUI or CLI tool — this is a library only
- Wrapping ffplay
- Supporting ffmpeg versions older than 5.0

## Decisions

### D1: Filter API — Immutable records, not reactive operators

Filters describe what ffmpeg does internally. Modeling them as R3 operators would suggest .NET processes the data — it doesn't. Filters are part of the pipeline description (records), not the event stream (Observable).

**Simple filters** use a builder on `OutputOptions` producing `IReadOnlyList<IVideoFilter>` / `IReadOnlyList<IAudioFilter>`:

```csharp
.Output("out.mp4", o => o
    .VideoFilter(f => f.Scale(1920, 1080).HorizontalFlip())
    .AudioFilter(f => f.LoudNorm()))
```

Each filter is a sealed record implementing `IVideoFilter` or `IAudioFilter` with a `Render()` method returning the ffmpeg filter string. `ArgumentBuilder` joins them with `,`.

**Complex filters** use a separate `FilterGraph` model — a DAG of labeled pads and filter chains:

```csharp
var graph = FilterGraph.Create()
    .Input(0, StreamType.Video, "base")
    .Input(1, StreamType.Video, "logo")
    .Chain("base", f => f.Scale(1920, 1080), "scaled")
    .Merge("scaled", "logo", f => f.Overlay(10, 10), "out");
```

`FilterGraph` is an immutable record. `ArgumentBuilder` serializes it to `-filter_complex "..."`.

**Alternative considered**: FFMpegCore-style string builders. Rejected because strings aren't testable or composable.

### D2: ffprobe — Separate entry point, shared runner pattern

`Ffprobe` is a separate static entry (not on `Ffmpeg`) because it runs a different binary. Internally it shares the process management pattern:

```
Ffprobe (static entry)
  → ProbeRunner (internal, runs ffprobe -print_format json)
    → ProbeResultParser (System.Text.Json → MediaInfo records)
```

`MediaInfo` is an immutable record tree. ffprobe output is JSON — use `System.Text.Json` source generators for AOT compatibility.

**Alternative considered**: Wrapping ffprobe into `FfmpegSession`. Rejected — different binary, different output format, different lifecycle.

### D3: Percentage progress — Composition, not coupling

Percentage progress requires knowing the total duration (from ffprobe). Rather than coupling ffprobe into the pipeline, the user composes:

```csharp
var info = await Ffprobe.AnalyseAsync("video.mp4");
session.Events
    .OfType<FfmpegEvent, ProgressEvent>()
    .Select(p => p.AsPercentage(info.Duration))  // extension method
    .Subscribe(pct => ...);
```

This keeps probing and encoding decoupled. `PercentageProgress` is an R3 operator/extension, not a pipeline record.

**Alternative considered**: `WithPercentageProgress(duration)` on the builder that auto-probes. Rejected — implicit I/O in a builder violates the pure-description principle.

### D4: Graceful cancellation — `q` then kill

FFmpeg responds to `q` on stdin by finalizing output (writing mp4 index, flushing buffers). Current implementation kills immediately, which can corrupt output.

New behavior: on cancellation, write `q` to stdin. Wait up to 5 seconds for exit. If still running, kill.

Requires `RedirectStandardInput = true` in `ProcessStartInfo`.

### D5: Pipe I/O — Named pipes on Windows, Unix pipes on Linux

Pipe I/O needs platform abstraction. FFMpegCore's approach: named pipes on Windows, anonymous pipes on Unix. We follow the same pattern.

Interfaces:
- `IPipeSource` — writes data to ffmpeg's input
- `IPipeSink` — reads data from ffmpeg's output

Built-in implementations: `StreamPipeSource`, `StreamPipeSink`.

The `PipeInput` and `PipeOutput` records hold the pipe name/path. `ProcessRunner` manages pipe lifecycle.

### D6: Codec enums — Static class with string constants, not C# enums

FFmpeg has hundreds of codecs. C# enums force recompilation to add one. Use static classes with string constants:

```csharp
public static class VideoCodec
{
    public const string LibX264 = "libx264";
    public const string LibX265 = "libx265";
    public const string Copy = "copy";
    // ...
}
```

The builder accepts `string` — constants provide discoverability, raw strings provide extensibility.

**Alternative considered**: C# enums with `[EnumMember]`. Rejected — closed set, breaks on new ffmpeg versions.

### D7: Phased implementation order

```
Phase 1: codec-encoding         (foundation — everything uses codecs)
Phase 2: ffprobe-analysis       (enables % progress, metadata-aware pipelines)
Phase 3: percentage-progress    (depends on Phase 2 duration)
Phase 4: time-control           (seek/trim, depends on input/output options)
Phase 5: simple-filters         (depends on Phase 1 codec infrastructure)
Phase 6: complex-filters        (depends on Phase 5 filter records)
Phase 7: pipe-io                (independent but complex — platform-specific)
Phase 8: multi-output           (depends on pipeline model extension)
Phase 9: stream-recording       (depends on URL input + time control)
Phase 10: metadata-chapters     (depends on Phase 2 ffprobe chapters)
Phase 11: subtitle-handling     (depends on Phase 5/6 filters + Phase 10 metadata)
Phase 12: hw-acceleration       (independent, extends codec options)
Phase 13: global-config         (independent, affects binary resolution)
Phase 14: ecosystem-extras      (grab bag, depends on various phases)
```

## Risks / Trade-offs

- **[Platform-specific pipe I/O]** → Named pipes on Windows differ significantly from Unix. Mitigation: Abstract behind `IPipeSource`/`IPipeSink`, test on both platforms in CI.
- **[ffprobe JSON schema instability]** → ffprobe output varies between versions. Mitigation: Parse defensively with nullable fields, keep `RawJson` escape hatch.
- **[Filter combinatorial explosion]** → ffmpeg has 400+ filters. Mitigation: Ship typed records for the top ~30, provide `CustomFilter(name, args)` escape hatch for the rest.
- **[API surface growth]** → 14 capabilities add ~50 new types. Mitigation: Consistent patterns (all records, all in predictable namespaces). Users learn one pattern, apply everywhere.
- **[Graceful cancellation race]** → Writing `q` after process exit throws. Mitigation: Check `HasExited` before writing, catch pipe errors.
