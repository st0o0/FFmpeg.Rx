## Context

FFmpeg.Rx v1-roadmap delivered all 14 feature phases but with a stringly-typed architecture: `OutputOptions` accumulates CLI flag strings, `FfmpegOutput` carries `string[] CustomArguments`, and codecs are raw strings. The filter system (Phase 5-6) already uses typed records — the redesign extends that pattern to the entire API.

Reference designs: WebSocket.Rx (interface-first, `Message` private-ctor record, C#14 extension blocks, `Payload` readonly struct) and FunkArr (enums over strings, Completed/Failed response records, separated type worlds, ArchUnitNET enforcement).

## Goals / Non-Goals

**Goals:**
- Every FFmpeg concept has a .NET type — no string bags
- Compile-time enforcement of domain rules (no CRF on Copy, no Preset on audio)
- Builder scoping: each builder type has only the methods that make sense
- Domain model testable without serialization — records are the truth
- Serialization testable without domain logic — ArgumentSerializer is pure
- Consumer mockability via IFfmpegSession interface
- Escape hatches at every level for features the typed API doesn't cover yet

**Non-Goals:**
- Runtime validation of codec/container compatibility (too version-dependent)
- Exhaustive codec/format constants (the `.Custom()` escape hatch covers the tail)
- Changing the event system (Observable<FfmpegEvent> stays)
- Changing the probing API (Ffprobe.AnalyseAsync stays)
- Changing the filter system design (already uses typed records — keep as-is)

## Decisions

### D1: Codec types — readonly record struct with static factories

```csharp
readonly record struct VideoCodec(string Name)
{
    public static VideoCodec H264 => new("libx264");
    public static VideoCodec Copy => new("copy");
    public static VideoCodec Custom(string name) => new(name);
}
```

`VideoCodec` and `AudioCodec` are different types at compile time. A method taking `VideoCodec` cannot receive an `AudioCodec`. The `Custom()` factory is the escape hatch for codecs we don't predefine.

**Alternative considered:** Sealed record class hierarchy (`sealed record H264Codec : VideoCodec`). Rejected — too many classes, too rigid for new ffmpeg versions, forces heap allocation.

**Alternative considered:** String constants in static classes (current design). Rejected — no type safety between video/audio codecs.

### D2: Copy vs Encode — disjoint CodecSpec

```csharp
abstract record CodecSpec;
sealed record StreamCopy : CodecSpec;
sealed record VideoEncode(VideoCodec Codec, VideoEncodingOptions Options) : CodecSpec;
sealed record AudioEncode(AudioCodec Codec, AudioEncodingOptions Options) : CodecSpec;
sealed record SubtitleEncode(SubtitleCodec Codec) : CodecSpec;
```

`StreamCopy` carries no encoding options. This is enforced by the type system, not runtime validation. The builder API makes this natural: `s.Copy()` returns void (terminal), `s.Encode(codec, e => ...)` gives an encoding builder.

### D3: Quality — disjoint sum type

```csharp
abstract record QualitySpec;
sealed record ConstantQuality(int Crf) : QualitySpec;
sealed record ConstantBitrate(Bitrate Rate) : QualitySpec;
```

CRF and bitrate are mutually exclusive in FFmpeg. Making them variants of a single type enforces this: `.Quality(Quality.Crf(23))` OR `.Quality(Quality.Bitrate(Bitrate.Mbps(4)))`, never both. The `Quality` static class provides the factories.

### D4: Value objects as readonly record struct

`Bitrate`, `Framerate`, `Preset`, `VideoProfile`, `Tune`, `PixelFormat`, `ContainerFormat` are all `readonly record struct` with:
- Private constructor
- Static factories for well-known values
- `Custom()` escape hatch where appropriate
- Value semantics (equality, comparison where it makes sense)

This follows the `Message.Create()` pattern from WebSocket.Rx.

### D5: Scoped builders — one type per context

Each builder exposes only the methods valid in its scope:

| Builder | Available methods |
|---------|-------------------|
| `PipelineBuilder` | `.Input()`, `.Output()`, `.Outputs()`, `.WithProgressReporting()` |
| `OutputBuilder` | `.Stream()`, `.VideoFilter()`, `.AudioFilter()`, `.FilterComplex()`, `.Container()`, `.TimeRange()`, `.Metadata()`, `.FastStart()`, `.MovFlags()`, `.NoOverwrite()`, `.CustomArg()` |
| `StreamBuilder` | `.Copy()`, `.Encode(VideoCodec)`, `.Encode(AudioCodec)`, `.Encode(SubtitleCodec)`, `.Metadata()`, `.Disposition()` |
| `VideoEncodingBuilder` | `.Quality()`, `.Preset()`, `.Profile()`, `.Tune()`, `.PixelFormat()`, `.Framerate()` |
| `AudioEncodingBuilder` | `.Bitrate()`, `.SampleRate()`, `.Channels()` |
| `TimeRangeBuilder` | `.Seek()`, `.Duration()`, `.StopTime()` |
| `MetadataBuilder` | `.Global()`, `.StripAll()`, `.CopyFrom()` |

No god objects. Each builder produces its corresponding record.

### D6: ArgumentSerializer — pattern matching on records

```csharp
internal static class ArgumentSerializer
{
    internal static string Serialize(PipelineSpec pipeline) { ... }
    
    // Decomposes into per-concept serializers:
    private static void SerializeInput(StringBuilder sb, InputSpec input) { ... }
    private static void SerializeOutput(StringBuilder sb, OutputSpec output) { ... }
    private static void SerializeStream(StringBuilder sb, StreamSpec stream) { ... }
    private static void SerializeCodec(StringBuilder sb, CodecSpec codec, StreamType type)
    {
        switch (codec)
        {
            case StreamCopy:
                // -c:v copy
                break;
            case VideoEncode ve:
                // -c:v {ve.Codec.Name} + serialize options
                break;
            case AudioEncode ae:
                // -c:a {ae.Codec.Name} + serialize options
                break;
        }
    }
}
```

Pattern matching on the record hierarchy. No string parsing, no flag name lookups. Each record type maps to a specific CLI output.

### D7: IFfmpegSession interface

```csharp
public interface IFfmpegSession : IAsyncDisposable
{
    Observable<FfmpegEvent> Events { get; }
    Observable<ProgressEvent> Progress { get; }
    Observable<LogEvent> Log { get; }
    Task<FfmpegResult> RunAsync(CancellationToken ct = default);
}
```

Following WebSocket.Rx's `IReactiveWebSocketClient` pattern. Consumers program against the interface. `FfmpegSession` implements it.

### D8: Three-layer architecture

```
Layer 1: Pipeline (records)     — what the user wants
Layer 2: Builder (fluent API)   — how the user says it
Layer 3: Serializer + Runner    — how it gets done
```

Each layer is independently testable. Layer 1 tests assert record values. Layer 2 tests assert that builders produce correct records. Layer 3 tests assert CLI string output from known records.

## Risks / Trade-offs

- **[Breaking change]** → Every consumer call changes. Mitigation: this IS the v1.0 — no backward compat needed yet. The v1-roadmap was a proof-of-concept.
- **[Builder nesting depth]** → `o.Stream(0, Video, s => s.Encode(H264, e => e.Crf(23)))` is 3 levels deep. Mitigation: each level is short and IDE-discoverable. Users can also construct records directly for complex cases.
- **[Value object proliferation]** → ~10 new struct types. Mitigation: all follow the same pattern, all are zero-allocation readonly record struct.
- **[Escape hatch leakage]** → `.CustomArg()` could become the default path for lazy users. Mitigation: make the typed API good enough that `.CustomArg()` is rarely needed.
