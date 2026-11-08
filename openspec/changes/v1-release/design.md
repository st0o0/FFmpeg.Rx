## Context

FFmpeg.Rx is architecturally clean after the api-redesign and architecture-cleanup changes. The typed model, CliWrap integration, and R3 event stream work. What's missing is packaging, a few high-value features, and comprehensive test coverage.

## Decisions

### D1: Snapshot as static convenience method

```csharp
await Ffmpeg.Snapshot("video.mp4", TimeSpan.FromSeconds(5), "thumb.jpg");
```

Internally builds: `-ss 5 -i video.mp4 -frames:v 1 -update 1 thumb.jpg`

Simple static method, no builder needed. Returns `Task<FfmpegResult>`.

### D2: Concat via concat demuxer

FFmpeg has two concat approaches: the concat demuxer (file list) and the concat filter. The demuxer is simpler and works for same-codec files:

```csharp
await Ffmpeg.Concat(["part1.mp4", "part2.mp4"], "full.mp4");
```

Internally creates a temp concat file and uses `-f concat -safe 0 -i list.txt -c copy`.

For different codecs, users use the existing filter graph API.

### D3: Percentage progress auto-wiring

```csharp
var session = Ffmpeg.Input("video.mp4")
    .WithProgressReporting()
    .Output("out.mp4", o => ...);

session.Progress.Subscribe(p => Console.WriteLine($"{p}"));
// Also: session.Events.OfType<PercentageProgressEvent>().Subscribe(...)
```

`WithProgressReporting()` already adds `-progress pipe:1`. To emit `PercentageProgressEvent`, we need the input duration. Two options:

**Option A:** Probe at session creation time (sync, blocks)
**Option B:** Probe lazily on first ProgressEvent (async, one-time cost)
**Option C:** User provides duration explicitly

Chosen: **Option C** with auto-probe fallback. `WithProgressReporting(TimeSpan? knownDuration = null)`. If null and a single file input exists, auto-probe via `Ffprobe.AnalyseAsync` before starting the process.

### D4: Coverage strategy

Three layers of coverage:

1. **Builder unit tests** — every builder method produces the correct record field value. These are fast, no ffmpeg needed. Target: every 0% builder to 100%.

2. **Serializer tests** — every record variant serializes to the correct CLI string. Already mostly covered (91%), fill gaps.

3. **Integration tests** — real ffmpeg for pipe I/O, tee, null output, multi-output. These verify the CliWrap wiring.

### D5: Packaging follows WebSocket.Rx pattern

- `global.json` pins SDK
- `packages.lock.json` committed, CI uses `--locked-mode`
- SourceLink via `Microsoft.SourceLink.GitHub`
- `dotnet-release-nuget.yml` from shared workflows
- `RELEASE_NOTES.md` for conventional changelog

### D6: C#14 extension blocks

Following WebSocket.Rx's `Extensions.cs` pattern:

```csharp
extension(IFfmpegSession session)
{
    public Observable<double> PercentageProgress(TimeSpan totalDuration) => ...
}
```

Adds composable R3 operators without polluting the interface.

## Risks

- **Auto-probe in WithProgressReporting** adds latency on first run. Mitigation: optional `knownDuration` parameter.
- **Concat demuxer** requires same-codec files. Document this clearly, point to filter graph for mixed codecs.
- **100% coverage** means testing error paths (FfprobeException, process start failure). Some are hard to trigger deterministically.
